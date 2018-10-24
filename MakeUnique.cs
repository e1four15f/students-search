/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 22.10.2018
 * Time: 20:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MakeUnique
{
	class Program
	{
		public static Dictionary<string, string[]> Filter(string[] entries, ref List<string> error_list){
			/*массив строк для разбора, лист для выписывания ошибок*/
			
			/*словарь <айди,массив контактов человека>*/
			Dictionary<string, string[]> person_contacts = new Dictionary<string, string[]>();
			
			/*лист для добавления правильной информации о пользователе*/
			List<string> skype_or_emails = new List<string>();
			
			/* строка: текущий_айди, предыдущий_айди
			 * для контроля строк одного человека, и добавления их в словарь
			 * только для текущего айди*/
			string current_id, previous_id;
			for(int i = 0; i < entries.Length - 1;){
				/* айди, как текущий, так и предыдущий, могут быть ошибочными:
				 * быть нулевыми, быть без ника и тп*/
				try{
					current_id = entries[i + 1].Substring(0,entries[i + 1].IndexOf(";"));
					previous_id = entries[i].Substring(0,entries[i].IndexOf(";"));
				}
				/* такие айди отлавливаем, выводим в консоль, добавляем в error_list
				 * тк некоторые могут быть и вполне верными, но предшествущий им айди -
				 * ошибочным*/
				catch(Exception e){
					Console.WriteLine("current_id: " + entries[i + 1]);
					Console.WriteLine("previous_id: " + entries[i]);
					error_list.Add(entries[i]);
					error_list.Add(entries[i + 1]);
					i++;
					continue;
				}
				/* если текущий и предыдущий айди равны - мы пока анализируем записи одного и того же пользователя*/
				if(current_id.Equals(previous_id)){
					/*вычленяем контакт и записываем его в skype_or_emails*/
					int data_begin = entries[i].IndexOf(";");
					string data = entries[i].Substring(data_begin + 1);
					skype_or_emails.Add(data);
					i++;
					current_id = entries[i].Substring(0,entries[i].IndexOf(";"));
				}
				else{
					
					/*person_contacts.Add() может выбить исключение, если запись в словаре уже есть*/
					try{
						/*последний контакт тоже нужно записать*/
						int data_begin = entries[i].IndexOf(";");
						string data = entries[i].Substring(data_begin + 1);
						skype_or_emails.Add(data);
						
						i++;
						/*добавляем все контакты в словарь*/
						person_contacts.Add(previous_id,skype_or_emails.ToArray());
						
						/*очищаем контакты уже записанного человека*/
						skype_or_emails.Clear();
						continue;
					}
				
					/*вывод отладочной информации и запись данных в error_list*/
					catch(Exception e){
						Console.WriteLine(i.ToString());
						string[] in_dict = null;
						System.Console.BackgroundColor = ConsoleColor.Red;
						System.Console.ForegroundColor = ConsoleColor.White;
						person_contacts.TryGetValue(previous_id, out in_dict);
						foreach(string string_in_dict in in_dict)
							Console.WriteLine(string_in_dict);
						
						System.Console.BackgroundColor = ConsoleColor.Blue;
						System.Console.ForegroundColor = ConsoleColor.White;
						
						foreach(string string_in_arr in skype_or_emails.ToArray()){
							Console.WriteLine(string_in_arr);
							error_list.Add("err_dict:" + previous_id +"|" + string_in_arr);
						}
						Console.ResetColor();
						continue;
					}
					
				}
			}
			
			return person_contacts;
		}
		
		public static string[] Load(string filename){
			
			return File.ReadAllLines(filename);
			
		}
		
		public static void Main(string[] args)
		{
			/* лист, где сохранятся ошибки
			   лист для айди, которых не нашли в проверенных
			   поток для записи в файл проверенных пользователей
			   поток для записи в файл ошибочных пользователей*/
			List<string> error_list = new List<string>();
			List<string> not_contains = new List<string>();
			StreamWriter unique_contacts = new StreamWriter("unique_contacts.txt");
			StreamWriter erroneous_ids = new StreamWriter("erroneous_ids.txt");
			
			
			Dictionary<string, string[]> person_contacts = Filter(Load("profiles_merged.txt"), ref error_list);
			string[] only_ids = Load("only_id.txt");
			
			for(int i = 0; i < only_ids.Length; i++){
				if(!person_contacts.ContainsKey(only_ids[i]))
					not_contains.Add(only_ids[i]);
			}
		
			for(int i = 0; i < person_contacts.Count; i++){
				if(person_contacts.ContainsKey(only_ids[i])){
					string[] contacts;
					person_contacts.TryGetValue(only_ids[i],out contacts);
					foreach(string contact in contacts)
						unique_contacts.WriteLine(only_ids[i] + ";" + contact);
				}
				else if(not_contains.Contains(only_ids[i])){
					erroneous_ids.WriteLine(only_ids[i]);
				}
			}
			unique_contacts.Flush();
			erroneous_ids.Flush();
		}
	}
}