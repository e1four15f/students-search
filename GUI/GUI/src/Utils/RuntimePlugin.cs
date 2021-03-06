﻿/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 05.11.2018
 * Time: 10:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using DB;
using System.Windows;
using System.IO;
namespace RuntimePlugin_ns
{
	public class RuntimePlugin
	{
		/*	В импортируемом dll плагине должен содержаться класс, содержащий
		 	имя подключаемой dll, и функция, так же содержащая имя dll.
		 	Если требуется вызывать цепочку функций, то это должна быть функция-обертка над остальными.
		 	Если класса или функции, содержащей имя длл не будет, то модуль выкинет ошибку.
		 	Поиск класса и функции не зависит от регистра их имени.
		 	class ExampleDLL, public void ExAmPlEDLL(List<Human> human),при скомпилированном Example.dll будут найдены.
		 	Функция-обертка должна принимать List<Human>, ничего больше*/
		
		Assembly dll;
		public string dll_name;
		public string DLL_NAME {get {return dll_name;}}
		public string dll_descr;
		public string DLL_DESCR {get {return dll_descr;}}
		
		
		//метод-обертка из dll
		public MethodInfo primary_method;
		//класс-обработчик из dll
		public object	  primary_instance;
		
		public RuntimePlugin(string path){
			dll = Assembly.LoadFile(path);
			int start = path.LastIndexOf("\\") + 1;
			
			if(!path.Contains("dll"))
				throw new Exception("This is not dll");
			try{
				dll_name = path.Substring(start, path.IndexOf(".") - start).ToLower();
			}
			catch(Exception){
				throw new Exception("Error occured in finding dll name");
			}
			
			GetFunction();
		}
		
		
		void GetFunction(){
			List<Type> exported_objects = new List<Type>();
			List<object> members = new List<object>();
			
			//проходим по всем экспортированным объектам
			foreach(Type exported_type in dll.GetExportedTypes()){
				
			//добавляем их в лист экспортированных объектов
				exported_objects.Add(exported_type);
				try{
			//и пытаемся получить объект экспортированного класса в лист экспортированных классов
					members.Add(Activator.CreateInstance(exported_type));
				}
				catch(Exception){
			//видимо это не класс, идем дальше	
					continue;
				}
			}
			//класс должен содержать название dll плагина, из всех выбираем такой
			primary_instance = members.Find( x => x.GetType().Name.ToLower().Contains(dll_name));
			
			
			//получаем методы класса
			MethodInfo[] methods = primary_instance.GetType().GetMethods();
			
			//выбираем из них тот, что содержит имя dll
			primary_method = methods.ToList<MethodInfo>().Find(x=> x.Name.ToLower().Contains(dll_name));
			
			try{
				dll_descr = (string)methods.ToList<MethodInfo>().Find(x=> x.Name.Contains("Description")).Invoke(primary_instance,new object[]{});
			}
			catch(Exception){}
			
			//методов не нашлось - выкидываем ошибку
			if(primary_method == null)
				throw new Exception("No method, contained name of dll was found.\n" +
									"Rename general method of your plugin to wherein contains dll name");
			
		}
		
		
		//вызов функции-обертки dll без возвращаемых значений
		public void Call(List<Human> humans){
			ParameterInfo[] parameters =  primary_method.GetParameters();
			string serialized_cllection = Newtonsoft.Json.JsonConvert.SerializeObject(humans);
			primary_method.Invoke(primary_instance, new object[]{serialized_cllection});
		}
		
		
		//вызов функции-обертки dll с возвращаемыми значениями
		public object CallWReturn(List<Human> humans){
			ParameterInfo[] parameters =  primary_method.GetParameters();
			string serialized_cllection = Newtonsoft.Json.JsonConvert.SerializeObject(humans);
			return primary_method.Invoke(primary_instance, new object[]{serialized_cllection});
		}
		
		public static void CreateTemplate(string path){
			string program = 
@"/*Шаблон плагина*/
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using HumanData;

namespace ChangeMe_Namespace
{
	public class NameMeAsDllFile_ClassName
	{
		/*	В импортируемом dll плагине должен содержаться класс, содержащий
		 	имя подключаемой dll, и функция, так же содержащая имя dll.
		 	Если требуется вызывать цепочку функций, то это должна быть функция-обертка над остальными.
		 	Если класса или функции, содержащей имя длл не будет, то модуль выкинет ошибку.
		 	Поиск класса и функции не зависит от регистра их имени.
		 	class ExampleDLL, public void ExAmPlEDLL(List<Human> human),при скомпилированном Example.dll будут найдены.
		 	Функция-обертка должна принимать List<Human>, ничего больше*/
		 
		/*Этот метод возвращает описание плагина*/
		public static string Description(){
			return "";
		}
		public static void NameMeAsDllFile(string serialized_list){
			List<Human> humans = JsonConvert.DeserializeObject<List<Human>>(serialized_list);
			
		}
	}
}";

            try
            {
                File.WriteAllText(path + @"\Program.cs", program);
                File.WriteAllText(path + @"\Human.cs", GUI.Properties.Resources.Human_class);
                MessageBox.Show("Файлы созданы");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Попробуйте заново", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (UnauthorizedAccessException)
            {

            }
		}
	}
}