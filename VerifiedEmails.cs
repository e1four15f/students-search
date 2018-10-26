/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 25.10.2018
 * Time: 21:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 #define MULTITHREAD

using System;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Miet_emails
{
	class Program
	{
		static string url = "https://www.abiturient.ru/forum/user/";
		static string fstart = "<title>Абитуриент.ру ".ToLower();
		static string fend = "</title>".ToLower();
		
		static void Download(ref ConcurrentQueue<string> ids,ref ConcurrentQueue<string> to_write){
			WebClient client = new WebClient();
			
			string id;
			string html;
			if(!ids.TryDequeue(out id))
				return;
			
			try{
				html = System.Text.Encoding.UTF8.GetString(client.DownloadData(url + id + "/")).ToLower();
				
				int start = html.IndexOf(fstart) + fstart.Length;
				int len = html.IndexOf(fend) - start;
				
				html = html.Substring(start,len);
			}
			catch(Exception e){
				return;
			}
			
			if(html.Contains("@"))
				to_write.Enqueue(html);
			
		}
		
		static bool Write(StreamWriter sw,ref ConcurrentQueue<string> to_write){
			/*В функцию передается: поток на запись, айди, словарь для многопоточности вида <айди,сайты>*/
			
			/*массив профилей на сайтах*/
			string profile;
			
			/*если возвращается false, то словарь занят - ждем через while, пока освободится*/
			to_write.TryDequeue(out profile);
			
			/*если сайтов нет, то запись не состоялась, возвращаем false*/
			if(profile == null)
				return false;

			/*сайты есть, пишем в файл все строки не равные null*/
			sw.WriteLine(profile);
			
			/*запись удачна, возвращаем true*/
			return true;
		}
		
		public static void Main(string[] args)
		{
			/*Создаем поток для файла, в который будем писать*/
			StreamWriter sw = new StreamWriter(@".\profiles.txt");
			
			/*получаем в массив ids все айдишники пользователей, связанных с миэтом*/
			
			
			/*Словарь для многопоточности вида <айди,сайты>*/
			ConcurrentQueue<string> to_write = new ConcurrentQueue<string>();
			ConcurrentQueue<string> ids_queue = new ConcurrentQueue<string>();
			
			int i = 0;
			#if(SKIP_ID)
			/*пропускаем n людей, чтобы сразу начать скан с последнего просканенного*/
			for(; i < ids.Length; i++)
				if(i == 2)
					break;
			#endif
			
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Blue;
			/* Каждая запись в файл будет сбрасывать буфер
			 * можно поставить в false - думаю возрастет использование оперативки, но
			 * увеличится скорость, и (скорее всего) исчезнут коллизии при многопоточной записи в файл
			 * которые редко, но встречаются при true*/
			sw.AutoFlush = false;
			
			/*Лист заданий для потоков*/
			List<Action> tasks = new List<Action>();
			
			/*Для каждого айди создаем поток обработки*/
			for(; i < 100000 ; i++)
				#if (!MULTITHREAD)
				{
					ids_queue.Enqueue(i.ToString());
					ThreadProc(sw,ref ids_queue,ref sites);
				}
				#endif
				
				#if (MULTITHREAD)
				try{
				/*100 заданий поместятся в лист заданий, а потом вызовутся*/
					if(i%103 != 0){
					/*добавляем данные в очередь*/
					ids_queue.Enqueue(i.ToString());
					/*добавляем задание в лист потоков*/
					tasks.Add(() => Download(ref ids_queue,ref to_write));
					tasks.Add(() => Write(sw,ref to_write));
					}
					else{
					/*запускаем все задания в многопотоке*/
						Parallel.Invoke(tasks.ToArray());
					/*они нам больше не нужны*/
						tasks.Clear();
					
						sw.Flush();
						ids_queue.Enqueue(i.ToString());
					/*но из-за i%101 мы пропустили 101й айди - надо его обработать*/
						tasks.Add(() =>  Download(ref ids_queue,ref to_write));
						tasks.Add(() => Write(sw,ref to_write));
					}
				}
			catch(Exception e)
			{
				System.Console.BackgroundColor = ConsoleColor.Magenta;
				System.Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine(e.InnerException.ToString());
				try{
					/*если нет автоматического сброса данных в файл, то сбрасывам буфер при ошибке*/
					if(!sw.AutoFlush)
						sw.Flush();
				}
				catch(Exception err){};
			}
			#endif
			if(tasks.Count != 0)
				Parallel.Invoke(tasks.ToArray());
			Console.ReadKey();
		}
	}
}