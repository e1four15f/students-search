/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 14.10.2018
 * Time: 10:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
 
 /*закомментить дефайн для того, чтобы убрать весь вывод в консоль*/
//#define DO_PRINT

 /*закомментить дефайн для того, чтобы убрать поиск в гугл*/
 //#define GOOGLE_IT

using System;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;



namespace miet_vk
{
	class Program
	{
		static string[] Download(string id){
			/*функция скачивания страницы пользователя*/
			WebClient client = new WebClient();
			/*кастомный id страницы*/
			string nickname;
			
			/*строка будет хранить имя и фамилию*/
			string name_surname;
				
			/*строка, скачиваемая WebClient*/
			string html;
			
			/*строка для поиска кастомного id в html, полученном от vk*/
			string get_nick = "href=\"https://vk.com/";
			
			/*url для поиска информации в гугле*/
			string google = "https://www.google.ru/search?q=";
			
			/*дефолтные сайты, для которые проверяются вне гугла*/
			string[] sites = {"https://pikabu.ru/@",
							  "https://www.twitch.tv/",
							  "https://twitter.com/",
							  "https://www.instagram.com/",
							  "https://ask.fm/",
							  "https://www.facebook.com/",
							  "https://habr.com/users/",};
			
			/*массив строк, игнорируемых при обработке информации, найденной в гугле*/
			string[] ignore = {"webcache","wiki"};
			
			/*лист с найденной информацией по нику*/
			List<string> profiles = new List<string>();
			
			/*пытаемся скачать страницу вк*/
			try{
				/*вк отдает страницы в UTF8, а .NET работает с UTF16*/
				html =  System.Text.Encoding.UTF8.GetString(client.DownloadData("https://vk.com/id" + id)).ToLower();
			}
			catch(Exception e){
				
			#if (DO_PRINT)
				
				/*Сообщаем об ошибке скачивания и возвращаем null*/
				System.Console.BackgroundColor = ConsoleColor.Red;
				System.Console.ForegroundColor = ConsoleColor.White;
				System.Console.WriteLine("Ошибка скачивания страницы vk");
				System.Console.ResetColor();
			#endif
			
				html = null;
			}
			
			/*страница вк не скачалась - выходим*/
			if(html == null)
				return null;
			
			/*вычленяем никнейм из html*/
			nickname = html.Substring(html.IndexOf(get_nick)+get_nick.Length);
			nickname = nickname.Substring(0,nickname.IndexOf("\" />"));
			
			/*вычленяем имя и фамилию человека в n_s_arr*/
			string get_n_s = "<h2 class=\"op_header\">";
			name_surname = html.Substring(html.IndexOf(get_n_s) + get_n_s.Length);
			name_surname = name_surname.Substring(0, name_surname.IndexOf("</h2>"));
			string[] n_s_arr = name_surname.Split(" ".ToCharArray());
			
			/*если в нике есть id, то, скорее всего это id**** , а по нему мало что можно найти*/
			if(nickname.Contains("id"))
				return null;
			
			/*обрабатываем дефолтные сайты*/
			for(int i = 0; i < sites.Length; i++){
				try{
					html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(sites[i] + nickname)).ToLower();
				}
				catch(Exception e){
					
				#if (DO_PRINT)
					/*Сообщаем об ошибке скачивания и переходим к следующей странице*/
					System.Console.BackgroundColor = ConsoleColor.Red;
					System.Console.ForegroundColor = ConsoleColor.White;
					System.Console.WriteLine("Ошибка скачивания страницы " + sites[i] + nickname);
					System.Console.ResetColor();
				#endif
					continue;
				}
				
				/*если на странице есть либо никнейм, либо имя, либо фамилия - добавляем в лист в виде:  айди_вк;профиль_дефолтного_сайта*/
				if ((html.Contains(nickname) || html.Contains(n_s_arr[0]) || html.Contains(n_s_arr[1])) && !html.Contains("ошибка")){
					
				#if (DO_PRINT)
					System.Console.BackgroundColor = ConsoleColor.DarkGreen;
					System.Console.ForegroundColor = ConsoleColor.White;
					System.Console.WriteLine("Страница " + sites[i] + nickname + " существует");
					System.Console.ResetColor();
				#endif
					profiles.Add(id + ";" + sites[i] + nickname);
				}
			}
			
			#if (GOOGLE_IT)
			/*обработчик запроса гугла*/
			try{
				html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(google + nickname)).ToLower();
				}
			catch(Exception e){
				
			#if (DO_PRINT)
			
				/*Сообщаем об ошибке скачивания и возвращаем null*/
				System.Console.BackgroundColor = ConsoleColor.Red;
				System.Console.ForegroundColor = ConsoleColor.White;
				System.Console.WriteLine("Ошибка скачивания страницы " + google + nickname);
				System.Console.ResetColor();
			
			#endif
				html = null;
			}
			
			
			/*гугл не сработал, возвращаем из функции то, что есть*/
			if(html == null)
				return profiles.ToArray();
			
			/*гугл сработал, обрабатываем первые три ссылки (самые релевантные), которые он предоставил*/
			for(int i = 0; i < 3; i++){
		
				/*вычленяем в googled_url ссылку из всего html*/
				string get_url = "/url?q=";
				html = html.Substring(html.IndexOf(get_url)+get_url.Length);
				string googled_url = html.Substring(0,html.IndexOf("&amp"));
			
				/*проверяем все игнорируемые строки, каждая игнорируемая строка НЕ УВЕЛИЧИВАЕТ счетчик релевантных ссылок*/
				for(int j = 0, index = 0; j < ignore.Length; j++){
					index = googled_url.IndexOf(ignore[j]);
					
					/*если индекс игнорируемой строки лежит в пределах 30 символов, то мы ее игнорируем
					  и идем вычленять следующую, перепрыгивая вывод и добавление в лист*/
						if(index < 30 && index != -1){
						--i;
						goto continue_wrapped_for;
					}
				}
			
			#if (DO_PRINT)
				System.Console.BackgroundColor = ConsoleColor.Blue;
				System.Console.ForegroundColor = ConsoleColor.White;
				System.Console.WriteLine("Нашли в гугле " + googled_url);
				System.Console.ResetColor();
			#endif
				
				/*загугленные строки будут помечаться ## перед собой, для более удобного парсинга*/
				profiles.Add("##"+ googled_url);
				
	continue_wrapped_for:
				continue;
			}
			
			#endif
			
			return profiles.ToArray();
		}
			
		static bool Write(StreamWriter sw,string id, ConcurrentDictionary<string,string[]> users_sites){	
			/*В функцию передается: поток на запись, айди, словарь для многопоточности вида <айди,сайты>*/
			
			/*массив профилей на сайтах*/
			string[] profiles;
			
			/*если возвращается false, то словарь занят - ждем через while, пока освободится*/
			users_sites.TryRemove(id,out profiles);
			
			/*если сайтов нет, то запись не состоялась, возвращаем false*/
			if(profiles == null)
				return false;

			/*сайты есть, пишем в файл все строки не равные null*/
			for(int i = 0; i < profiles.Length; i++){
				if(profiles[i] != null)
					sw.WriteLine(profiles[i]);
			}
			
			/*запись удачна, возвращаем true*/
			return true;
		}
		
		public static bool ThreadProc(StreamWriter sw,ref ConcurrentQueue<string> ids, ref ConcurrentDictionary<string,string[]> users_sites){
			/*В функцию передается: поток на запись, очередь айди, словарь для многопоточности вида <айди,сайты>*/
			
			/*пытаемся достать в id первый элемент очереди*/
			string id;
			ids.TryDequeue(out id);
			/*скачиваем и анализируем данные по айди*/
			string[] results = Download(id);
			
			/*если возвращается false, то в словаре уже есть такое значение*/
			if(!users_sites.TryAdd(id,results))
				return false;
			
			/*пишем полученные данные в файл*/
			Write(sw,id,users_sites);
			return true;
		}
		
		
		/*рисует прогресс программы в консоли*/
		public static void Progress(int current, int full){
			Console.Clear();
			Console.BackgroundColor = ConsoleColor.Green;
			Console.ForegroundColor = ConsoleColor.White;
			for(int i = 0; i < (int)(((float)current/(float)full)*100); i++)
				Console.Write("{0,3}",i);
			Console.BackgroundColor = ConsoleColor.Red;
			for(int i = (int)(((float)current/(float)full)*100); i < 100; i++)
				Console.Write("{0,3}",i);
			Console.ResetColor();
			Console.WriteLine();
		}
		
		public static void Main(){
			
			/*Создаем поток для файла, в который будем писать*/
			StreamWriter sw = new StreamWriter(@".\profiles.txt");
			
			/*получаем в массив ids все айдишники пользователей, связанных с миэтом*/
			string[] ids = File.ReadAllLines("only_id.txt");
			
			
			/*Словарь для многопоточности вида <айди,сайты>*/
			ConcurrentDictionary<string,string[]> sites = new ConcurrentDictionary<string,string[]>();
			ConcurrentQueue<string> ids_queue = new ConcurrentQueue<string>();
			
			int i = 0;
			#if(SKIP_ID)
			/*пропускаем n людей, чтобы сразу начать скан с последнего просканенного*/
			for(; i < ids.Length; i++)
				if(ids[i].Equals("328637878"))
					break;
			#endif
			
			
			/* Каждая запись в файл будет сбрасывать буфер
			 * можно поставить в false - думаю возрастет использование оперативки, но
			 * увеличится скорость, и (скорее всего) исчезнут коллизии при многопоточной записи в файл
			 * которые редко, но встречаются при true*/
			sw.AutoFlush = true;
			
			/*Лист заданий для потоков*/
			List<Action> tasks = new List<Action>();
			
			/*Для каждого айди создаем поток обработки*/
			for(; i < ids.Length; i++)
				try{
				/*100 заданий поместятся в лист заданий, а потом вызовутся*/
					if(i%101 != 0){
					/*добавляем данные в очередь*/
						ids_queue.Enqueue(ids[i]);
					/*добавляем задание в лист потоков*/
						tasks.Add(() => ThreadProc(sw,ref ids_queue,ref sites));
					}
					else{
						Progress(i,ids.Length);
					/*запускаем все задания в многопотоке*/
						Parallel.Invoke(tasks.ToArray());
					/*они нам больше не нужны*/
						tasks.Clear();
					
						ids_queue.Enqueue(ids[i]);
					/*но из-за i%101 мы пропустили 101й айди - надо его обработать*/
						tasks.Add(() => ThreadProc(sw,ref ids_queue,ref sites));
					}
				}
			catch(Exception e)
			{
				System.Console.BackgroundColor = ConsoleColor.Magenta;
				System.Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine(e.InnerException.ToString());
				Console.Beep();
				try{
					/*если нет автоматического сброса данных в файл, то сбрасывам буфер при ошибке*/
					if(!sw.AutoFlush)
						sw.Flush();
				}
				catch(Exception err){};
			}
			
			
			sw.Flush();
			/*нажмите кнопочку и закройте программу*/
			System.Console.BackgroundColor = ConsoleColor.Red;
			System.Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Нажмите на любую кнопочку чтобы закрыть программу");
			System.Console.ReadKey();
		}
	}
}