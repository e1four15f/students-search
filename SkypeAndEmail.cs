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

 /*закомментить дефайн для того, чтобы убрать пропуск просканенных айди*/
 //#define SKIP_ID
 
using System;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using MinimalisticTelnet;


namespace SkypeAndEmail
{
	struct VK{
		static public string foaf = "https://vk.com/foaf.php?id=";
		static public string full = "https://vk.com/id";
		
		
		/*строка для поиска кастомного id в html, полученном от vk*/
		static public string get_nick = "href=\"https://vk.com/";
	}
	
	struct User{
		/*кастомный id страницы*/
		public string nickname	{get;set;}
			
		public string skype		{get;set;}
			
		/*массив строк будет хранить имя и фамилию*/
		public string[] name_surname {get;set;}
		
	}
	
	class Program
	{
		static string[] Download(string id){
			/*функция скачивания страницы пользователя*/
			WebClient client = new WebClient();
			
			User user = new User();
			user.nickname = null;
			user.skype = null;
			
			/*строка, скачиваемая WebClient*/
			string html;
			
			/*url для поиска информации в гугле*/
			string google = "https://www.google.ru/search?q=";
			
			/*дефолтные провайдеры почты*/
			Tuple<string,string>[] providers = {	new Tuple<string,string>("@yandex.ru","mx.yandex.ru"),
													new Tuple<string,string>("@gmail.com","gmail-smtp-in.l.google.com"),
													new Tuple<string,string>("@mail.ru", "mxs.mail.ru"),
													new Tuple<string,string>("@protonmail.com", "mailsec.protonmail.ch"),
													new Tuple<string,string>("@yahoo.com", "mta7.am0.yahoodns.net"),
													new Tuple<string,string>("@rambler.ru","inmx.rambler.ru")};
													
			
			/*лист с найденной информацией по нику*/
			List<string> profiles = new List<string>();
			
			/*пытаемся скачать страницу вк*/
			try{
				user.skype = client.DownloadString(VK.foaf + id).ToLower();
				
				/*вычленяем skype*/
				if(user.skype.Contains("<foaf:skypeid>")){
					user.skype = user.skype.Substring(user.skype.IndexOf("<foaf:skypeid>") + "<foaf:skypeid>".Length);
					user.skype = user.skype.Substring(0,user.skype.IndexOf("</foaf:skypeid>"));
					profiles.Add(id + ";#" + user.skype);
				}
				else
					user.skype = null;
				
				
				html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(VK.full + id)).ToLower();
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
			
			/*страницы вк не скачались - выходим*/
			if(html == null && user.skype == null)
				return null;
			
			/*вычленяем никнейм из html*/
			user.nickname = html.Substring(html.IndexOf(VK.get_nick) + VK.get_nick.Length);
			user.nickname = user.nickname.Substring(0,user.nickname.IndexOf("\" />"));
			
			/*вычленяем имя и фамилию человека в user.name_surname*/
			string get_n_s = "<h2 class=\"op_header\">";
			get_n_s = html.Substring(html.IndexOf(get_n_s) + get_n_s.Length);
			get_n_s = get_n_s.Substring(0, get_n_s.IndexOf("</h2>"));
			user.name_surname = get_n_s.Split(" ".ToCharArray());
			
			/*проверяем всех провайдеров, и если почта удовлетворяет всем требованиям - заносим в лист*/
			for(int i = 0; i < providers.Length; i++){
				string email = user.nickname + providers[i].Item1;
				if(IsValidEmail(email,providers[i].Item2) && user.nickname != null && !user.nickname.Contains("id"))
					profiles.Add(id + ";" + email);
				
				email = user.skype + providers[i].Item1;
				
				if(user.skype != null && IsValidEmail(email,providers[i].Item2) )
					profiles.Add(id + ";" + email);
			}
			
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
		
		static public bool IsValidEmail(string email,string provider){
			
			TelnetConnection telnet;
			string response;
				
			try{
				telnet = new TelnetConnection(provider,25);
			}
			catch(Exception e){
				return false;
			}
			
			if(!telnet.IsConnected)
				return false;
			
			try{
				telnet.WriteLine("helo hi");
				
				/*чистим буфер ответов телнета*/
				telnet.Read();
				
				telnet.WriteLine("mail from: <testing_for_university_project@university.com>");
				telnet.Read();
				
				telnet.WriteLine("rcpt to: <" + email + ">");
				response = telnet.Read().ToLower();
				
				telnet.WriteLine("quit");
				telnet.Read();
			}
			catch(Exception e){
				return false;
			}
			/*250 - адресат есть, 550 - нет*/
			if(response.Contains("250") && !response.Contains("550"))
				return true;
			else
				return false;
		}
	}
}