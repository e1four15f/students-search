/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 07.10.2018
 * Time: 20:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

 
 /*закомментить дефайн для того, чтобы убрать весь вывод в консоль*/
//#define DO_PRINT

 /*закомментить дефайн для того, чтобы убрать поиск в гугл*/
 //#define GOOGLE_IT

 /*закомментить дефайн для того, чтобы убрать пропуск просканенных айди*/
 //#define SKIP_ID
 
using System;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace GoogleReq
{
	public delegate bool Searcher(string query, string id, string nickname, ref List<string> profiles);
	        /*
	         	Функции обращения к поисковику одинаковые - под капотом:
	         	1. Получение страницы по запросу (query + nickname)
	         	2. Вычленение ссылок из полученной страницы
	         	3. Проверка ссылок на мусор и т.п
	         	4. Запись в лист профилей
	         	5. Возвращает true если все прошло удачно
	         	
	         	Так же поисковикам предоставляется User-agent через client.Headers.Add ("user-agent" ...
	         	чтобы они думали, что запрос идет от браузера.
	         	
	         	В некоторых местах, возможно, стоит заменить client.DownloadString на строку в комментариях
	         	хотя скачивание строки html через DownloadString не корежит латинские буквы, кириллица будет испорчена (но она нам и не нужна)
	         	
	         	Так же строку 
								for(int i = 0; i < sites.Length; i++){
				возможно стоит изменить на while, тк вычленение сайтов иногда (возможно часто) проглатывает мусор, но нужно помнить,
				что while сильно замедлит программу
	         */
	        
	class Program
	{
		/*замок для потоков*/
		static object profile_locker;
		
		 /*сайты, на которые мы ориентируемся*/
        static string[] sites = {"pikabu.ru",
						  "twitch.tv",
						  "twitter.com",
						  "instagram.com",
						  "steamcommunity.com",
						  "ask.fm",
						  "facebook.com",
						  "habr.com",
						  "joyreactor.cc",
        				  "4pda",
        				  "forum"};
		 
        /*массив строк, игнорируемых при обработке информации, найденной в гугле*/
        static string[] ignore = { "webcache", "wiki", "google", "bing", "yandex", "rambler", "microsoft" };
		
        
		static bool Google	(string query, string id, string nickname, ref List<string> profiles){
        	
            WebClient client = new WebClient();
            client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string html;
			try{
				html =  client.DownloadString(query + nickname).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();
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
        	
            if(html == null)
            	return false;
            		
			for(int i = 0; i < sites.Length; i++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "/url?q=";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("&amp"));

            foreach(string dnw2contain in ignore)
            if (googled_url.Contains(sites[i]) && !googled_url.Contains(dnw2contain))
                if (!profiles.Contains(id + ";" + googled_url)){
            	lock(profile_locker)
                    profiles.Add(id + ";" + googled_url);

#if (DO_PRINT)
                    System.Console.BackgroundColor = ConsoleColor.Blue;
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine("Нашли в гугле " + googled_url);
                    System.Console.ResetColor();
#endif
                }
            }
            
            return true;
		}
		
		static bool Bing	(string query, string id, string nickname, ref List<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string html;
			try{
				html =  client.DownloadString(query + nickname).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();
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
        	
            if(html == null)
            	return false;
        	
			for(int i = 0; i < sites.Length; i++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "<a href=\"";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("\""));

            foreach(string dnw2contain in ignore)
            if (googled_url.Contains(sites[i]) && !googled_url.Contains(dnw2contain))
                if (!profiles.Contains(id + ";" + googled_url)){
            	lock(profile_locker)
                    profiles.Add(id + ";" + googled_url);

#if (DO_PRINT)
                    System.Console.BackgroundColor = ConsoleColor.Blue;
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine("Нашли в гугле " + googled_url);
                    System.Console.ResetColor();
#endif
                }
            }
            
            return true;
		}
		
        static bool Yandex	(string query, string id, string nickname, ref List<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string html;
			try{
            	html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(google + nickname)).ToLower();
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
        	
            if(html == null)
            	return false;
            
            /*пропустить джаваскрипт*/
            html = html.Substring(html.IndexOf("</script><div class=\""));
            
			for(int i = 0; i < sites.Length; i++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "target=_blank href=\"";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("\""));

            foreach(string dnw2contain in ignore)
            if (googled_url.Contains(sites[i]) && !googled_url.Contains(dnw2contain))
                if (!profiles.Contains(id + ";" + googled_url)){
            	lock(profile_locker)
                    profiles.Add(id + ";" + googled_url);

#if (DO_PRINT)
                    System.Console.BackgroundColor = ConsoleColor.Blue;
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine("Нашли в гугле " + googled_url);
                    System.Console.ResetColor();
#endif
                }
            }
            return true;
		}
        
        static bool Rambler	(string query, string id, string nickname, ref List<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string html;
			try{
				html =  client.DownloadString(query + nickname).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();
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
        	
            if(html == null)
            	return false;
            
			for(int i = 0; i < sites.Length; i++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "href=\"";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("\""));

            foreach(string dnw2contain in ignore)
            if (googled_url.Contains(sites[i]) && !googled_url.Contains(dnw2contain))
                if (!profiles.Contains(id + ";" + googled_url)){
            	lock(profile_locker)
                    profiles.Add(id + ";" + googled_url);

#if (DO_PRINT)
                    System.Console.BackgroundColor = ConsoleColor.Blue;
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine("Нашли в гугле " + googled_url);
                    System.Console.ResetColor();
#endif
                }
            }
            return true;
		}
        
		static string[] Download(string id)
        {
            /*функция скачивания страницы пользователя*/
            WebClient client = new WebClient();
           //
            /*кастомный id страницы*/
            string nickname;

            /*строка, скачиваемая WebClient*/
            string html;

            /*строка для поиска кастомного id в html, полученном от vk*/
            string get_nick = "<ya:URI rdf:resource=\"http://vk.com/".ToLower();
            
            /*строки запроса для поисковиков*/
            string[] search_queries = {"https://www.google.ru/search?q=",
            						 "https://www.yandex.ru/search/?lr=213&text=",
            						 "https://www.bing.com/search?q=",
            						 "https://nova.rambler.ru/search?query="};
			          


            /*лист с найденной информацией по нику*/
            List<string> profiles = new List<string>();
            

            /*пытаемся скачать страницу вк*/
            try
            {
                /*.NET работает с UTF16, не забывайте!*/
                html = client.DownloadString("https://vk.com/foaf.php?id=" + id).ToLower();
            }
            catch (Exception e)
            {

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
            if (html == null)
                return null;

            /*вычленяем никнейм из html*/
            html = html.Substring(html.IndexOf(get_nick) + get_nick.Length);
            nickname = html.Substring(0, html.IndexOf("\"/>"));

            
            /*если в нике есть id и цифры, то, скорее всего это id**** , а по нему мало что можно найти*/
            if (nickname.Contains("id") && nickname.Any(char.IsDigit))
            	return null;
			
            /*Словарь для хранения функций, которые скачивают данные из определенного поисковика*/
            Dictionary<int, Searcher> call_search =  new Dictionary<int, Searcher>();
            call_search.Add(0, new Searcher(Google));
            call_search.Add(1, new Searcher(Yandex));
            call_search.Add(2, new Searcher(Bing));
            call_search.Add(3, new Searcher(Rambler));
            
            /*В этот делегат достанем значение из словаря*/
            Searcher current_searcher;
            
            /* Уверен, можно было бы придумать крутой алгоритм для максимизирования запросов к одному поисковику
               но я просто использую рандом, чтобы им не надоедать. обращаясь к случайному поисковику*/
            int dice;
            do{
            	dice = new Random().Next(0,3);
            	call_search.TryGetValue(dice,out current_searcher);
            }while(!current_searcher(search_queries[dice], id, nickname, ref profiles));

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
			if(users_sites.TryAdd(id,results))
				return false;
			
			/*пишем полученные данные в файл*/
			Write(sw,id,users_sites);
			return true;
		}
		
		
		/*рисует прогресс программы в консоли*/
		public static void Progress(int current, int full){
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
			
			profile_locker = new object();
			
			/*Создаем поток для файла, в который будем писать*/
			StreamWriter sw = new StreamWriter(@".\profiles_googled.txt");
			
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
					if(i%2 != 0){
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