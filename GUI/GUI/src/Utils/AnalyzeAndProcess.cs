/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 27.10.2018
 * Time: 19:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using MinimalisticTelnet;
using Newtonsoft.Json;

using DB;

namespace Utils
{
	abstract class AnalyzeData
	{
		
		public static bool CheckAge(int bday_year, int graduation_year){
            if (!bday_year.Equals(default(DateTime)) && !graduation_year.Equals(default(DateTime)))
                if (graduation_year - bday_year < 20)
					return true;
			return false;
		}
		
		public static bool CheckFriends(HashSet<Human> friends){
			int count = 0;
			foreach(Human friend in friends){
				if(friend.Plausibility > 10)
					count++;
				if(count > 5)
					return true;
			}
			return false;
		}
		
		public static bool CheckArrival(string arrival){
			//TODO вставить проверку через апи/веб_клиент, что паблик, из которого он пришел, относится к миэту
			
			return false;
		}
		
	}


	public delegate bool Searcher(string query, string nickname, string user_agent, ref HashSet<string> profiles);
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
				
                в foreach(string dnw2contain in ignore) проверяется, содержит ли ссылка слова, с которыми не надо включать в лог
                в for(int i = 0; i < sites.Length; i++) проверяется, содержит ли ссылка слова, с которыми НУЖНО включать в лог
               
	         */
	        
	abstract class ProcessData{
	    //TODO сделать методы для обращения к поставляемой с софтом БД
	    
		public static HashSet<string> MostLikelySites(Human human){
			/*Проверяет наиболее популярные сайты, на предмет существования там аккаунта человека*/
			
			/*функция скачивания страницы пользователя*/
			WebClient client = new WebClient();
				
			/*строка, скачиваемая WebClient*/
			List<string> html = new List<string>();
			
			
			/*дефолтные сайты, для которые проверяются вне гугла*/
			string[] sites = {"https://pikabu.ru/@",
							  "https://twitter.com/",
							  "https://www.instagram.com/",
							  "https://ask.fm/",
							  "https://www.facebook.com/",
							  "https://habr.com/users/"};
			
			
			/*лист с найденной информацией*/
			HashSet<string> profiles = new HashSet<string>();
			
			/*обрабатываем дефолтные сайты*/
			for(int i = 0; i < sites.Length; i++){
				try{
					if(human.domain != null && 
					   (!human.domain.Substring(0, 2).Equals("id") && human.domain.Substring(0, human.domain.Length -1).Any(char.IsDigit)))
						html.Add(System.Text.Encoding.UTF8.GetString(client.DownloadData(sites[i] + human.domain)).ToLower());
					
					if(human.instagram != null)
						html.Add(System.Text.Encoding.UTF8.GetString(client.DownloadData(sites[i] + human.instagram)).ToLower());
					if(human.skype != null)
						html.Add(System.Text.Encoding.UTF8.GetString(client.DownloadData(sites[i] + human.skype)).ToLower());
					if(human.twitter != null)
						html.Add(System.Text.Encoding.UTF8.GetString(client.DownloadData(sites[i] + human.twitter)).ToLower());
				}
				catch(Exception){
					continue;
				}
				
				/*если на странице есть либо никнейм, либо имя, либо фамилия - добавляем в лист в виде:  айди_вк;профиль_дефолтного_сайта*/
				foreach(string code in html){
					if (human.domain != null && code.Contains(human.domain))
						profiles.Add(sites[i] + human.domain);
						
					if (human.instagram != null && code.Contains(human.instagram))
						profiles.Add(sites[i] + human.instagram);
					
					if (human.skype != null && code.Contains(human.skype))
						profiles.Add(sites[i] + human.skype);
					
					if (human.twitter != null && code.Contains(human.twitter))
						profiles.Add(sites[i] + human.twitter);
				}
			}
			return profiles;
		}
		
			/*Использовать в связке с GetEmails*/
	    public static string[] GenerateAddr(Human human){

            if (human.domain.Substring(0, 2) == "id"
                && human.domain.Substring(3, human.domain.Length - 3).Any(char.IsDigit))
            {
                return new string[0];
            }

			/*NOT TESTED*/
			HashSet<string> addresses = new HashSet<string>();
            HashSet<string> dates = new HashSet<string>();
			addresses.Add(human.domain);
			addresses.Add(human.social.instagram);
			addresses.Add(human.social.facebook);
			addresses.Add(human.social.livejournal);
			addresses.Add(human.social.twitter);
			addresses.Add(human.social.skype);

			dates.Add(human.bdate.Month.ToString() + "_" + human.bdate.Year.ToString());
			dates.Add(human.bdate.Month.ToString() + "." + human.bdate.Year.ToString());
			dates.Add(human.bdate.Month.ToString() + "-" + human.bdate.Year.ToString());
			dates.Add(human.bdate.Month.ToString() + human.bdate.Year.ToString());
			dates.Add(human.bdate.Month.ToString() + (human.bdate.Year % 100).ToString());
			dates.Add(human.bdate.Month.ToString());
			dates.Add((human.bdate.Year % 100).ToString());
			dates.Add(human.bdate.Year.ToString());

            addresses.Remove(null);

			for (int i = 0, len = addresses.Count; i < len; i++)
            { 
				foreach(string date in dates)
                {
                    addresses.Add(addresses.ElementAt(i) + date);
                    addresses.Add(addresses.ElementAt(i) + "_" + date);
                    addresses.Add(addresses.ElementAt(i) + "." + date);
                    addresses.Add(addresses.ElementAt(i) + "-" + date);
			    }
                addresses.RemoveWhere(x => x.Length < 5);
            }
			return addresses.ToArray<string>();
		}
	    
			/*Использовать в связке с GenerateAddr*/
        public static HashSet<string> GetEmails(string address)
        {
			/*Возвращает, если есть, список возможных имэйлов пользователя*/
			WebClient client = new WebClient();
			
			/*строка, скачиваемая WebClient*/
			//string html;
			
			/*дефолтные провайдеры почты*/
			Tuple<string,string>[] providers = {	new Tuple<string,string>("@yandex.ru","mx.yandex.ru"),
													new Tuple<string,string>("@gmail.com","gmail-smtp-in.l.google.com"),
											//		new Tuple<string,string>("@mail.ru", "mxs.mail.ru"),
													new Tuple<string,string>("@protonmail.com", "mailsec.protonmail.ch"),
											//		new Tuple<string,string>("@yahoo.com", "mta7.am0.yahoodns.net"),
													new Tuple<string,string>("@rambler.ru","inmx.rambler.ru")};
													
			
			/*лист с найденной информацией по нику*/
			HashSet<string> profiles = new HashSet<string>();
			
			/*проверяем всех провайдеров, и если почта удовлетворяет всем требованиям - заносим в лист*/
			for(int i = 0; i < providers.Length; i++){
				string email = address + providers[i].Item1;
				if(IsValidEmail(email,providers[i].Item2) && address != null)
					profiles.Add(email);
			}
			
			return profiles;
		}
		
		static bool IsValidEmail(string email,string provider){
			
			TelnetConnection telnet;
			string response;
				
			try{
				telnet = new TelnetConnection(provider,25);
			}
			catch(Exception){
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
			catch(Exception){
				return false;
			}
			/*250 - адресат есть, 550 - нет*/
			if(response.Contains("250") && !response.Contains("550"))
				return true;
			else
				return false;
		}
		
		
		/********************************************************************/
		
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
		
       	
	
		static bool Google	(string query, string nickname, string user_agent, ref HashSet<string> profiles){
        	
            WebClient client = new WebClient();
            client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
				html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();//client.DownloadString(query + nickname).ToLower(); //
			}
			catch(Exception){
				html = null;
			}
        	
            if(html == null){
            	return false;
            }
            		
			for(int j = 0; j < 20; j++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url;
			if(html.Contains("/url?url="))
				get_url = "/url?url=";
			else
				get_url = "/url?q=";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("&amp"));

			foreach(string dnw2contain in ignore){
				if(googled_url.Contains(dnw2contain))
					goto upper_for;
            }
			for(int i = 0; i < sites.Length; i++){
				if (googled_url.Contains(sites[i]))
					if (!googled_url.Contains("#")){
							lock(profiles)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            
            return true;
		}
		
		static bool Bing	(string query, string nickname, string user_agent, ref HashSet<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
            	html = System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower(); //client.DownloadString(query + nickname).ToLower();;
				}
			catch(Exception){
				html = null;
			}
            
        	if(html == null){
            	return false;
            }
			for(int j = 0; j < 20; j++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "<a href=\"";
			html = html.Substring(html.IndexOf(get_url) + get_url.Length);
			if(html.Contains("не удалось"))
				return false;
			string googled_url = html.Substring(0,html.IndexOf("&amp"));

			foreach(string dnw2contain in ignore){
				if(googled_url.Contains(dnw2contain))
					goto upper_for;
            }
			for(int i = 0; i < sites.Length; i++){
				if (googled_url.Contains(sites[i]))
		                if (!googled_url.Contains("#")){
		            	lock(profiles)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            
            return true;
		}
		
        static bool Yandex	(string query, string nickname, string user_agent, ref HashSet<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
            	html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(google + nickname)).ToLower();
			}
			catch(Exception){
				html = null;
			}
        	if(html == null){
            	return false;
            }
            
            if(html.Contains("нам очень жаль, но"))
            	return false;
            /*пропустить джаваскрипт*/
            html = html.Substring(html.IndexOf("</script><div class=\""));
            
			for(int j = 0; j < 20; j++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "target=_blank href=\"";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("&amp"));

			foreach(string dnw2contain in ignore){
				if(googled_url.Contains(dnw2contain))
					goto upper_for;
            }
			for(int i = 0; i < sites.Length; i++){
				if (googled_url.Contains(sites[i]))
		                if (!googled_url.Contains("#")){
		            	lock(profiles)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            return true;
		}
        
        static bool Rambler	(string query, string nickname, string user_agent, ref HashSet<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
				html =  client.DownloadString(query + nickname).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();
			}
			catch(Exception){
				html = null;
			}
        	if(html == null){
            	return false;
            }
            
			for(int j = 0; j < 20; j++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "href=\"";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("&amp"));

			foreach(string dnw2contain in ignore){
				if(googled_url.Contains(dnw2contain))
					goto upper_for;
            }
			for(int i = 0; i < sites.Length; i++){
				if (googled_url.Contains(sites[i]))
		                if (!googled_url.Contains("#")){
		            	lock(profiles)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            return true;
		}
        
		public static HashSet<string> SearchInNet(string to_search, List<string> user_agents)
        {
            
            /*строки запроса для поисковиков*/
            string[] search_queries = {"https://www.google.ru/search?q=",
            						 "https://www.yandex.ru/search/?lr=213&text=",
            						 "https://www.bing.com/search?q=",
            						 "https://nova.rambler.ru/search?query="};

            /*лист с найденной информацией по нику*/
            HashSet<string> profiles = new HashSet<string>();
           
			
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
            string user_agent;
            do{
            	dice = new Random().Next(0,3);
            	call_search.TryGetValue(dice,out current_searcher);
            	
            	int random_user_agent = new Random().Next(0,user_agents.Count);
            	/*достаем в user_agent случайный user-agent из листа*/
            	user_agent = user_agents[random_user_agent];
            }while(!current_searcher(search_queries[dice], to_search, user_agent, ref profiles));

            for (int i = 0; i < profiles.Count; i++)
                /*выкидывает из profles url`ы, содержащие какой-нибудь мусор*/
                if (!Uri.IsWellFormedUriString(profiles.ElementAt(i), UriKind.Absolute))
                    profiles.Remove(profiles.ElementAt(i));
             
            return profiles;
        }
		
		
		/********************************************************************/
		
		//делегат для вычленения данных
		delegate string IsolateData(string start, string end, ref string origin_text);
		
		//структура для собрания информации из контактов
		public struct ShallowHuman{
			public string id;
			public string description;
			public string extra_info;
		}
		
		public static List<ShallowHuman> GetContacts(string group_url){
			//функция получает на вход адрес группы, а на выходе выдает лист с контактами группы
			WebClient client = new WebClient();
			client.Headers.Add ("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.57 Safari/537.17 QIHU 360EE");

			string html;
			string cycles;
			string 					filter2contacts = "<aside aria-label=\"Контакты\">".ToLower();
			Tuple<string, string> 	filter2count	= new Tuple<string, string>("<span class=\"header_count fl_l\">","</span>");
			string				 	filter_person	= "<div class=\"fl_l thumb\">";
			Tuple<string, string> 	filter_id		= new Tuple<string, string>("<div class=\"people_name\"><a href=\"/","\">");
			Tuple<string, string>	filter_descr	= new Tuple<string, string>("<div class=\"people_desc\">", "</div>");
			Tuple<string, string>	filter_extra	= new Tuple<string, string>("<div class=\"people_extra\">","</div>");
			
			//анонимная функция для вычленения данных
			IsolateData isolate = (string start_str, string end_str, ref string origin_text) =>{
					int end;
					try{
						origin_text = origin_text.Substring(origin_text.IndexOf(start_str) + start_str.Length);
						end = origin_text.IndexOf(end_str);
						string isolated = origin_text.Substring(0, end);
						
						//<br> будет заменен на ;
						return isolated.Replace("<br>", ";");
					}
					catch(Exception){
						return null;
					}
			};
			
			
			try{
				//скачиваем страницу группы
				html = client.DownloadString(group_url).ToLower();
				//и скипаем данные до контактов
				html = html.Substring(html.IndexOf(filter2contacts));
			}
			catch(Exception){
				return null;
			}
			
			try{
				//получаем кол-во людей в контактах
				cycles = html.Substring(html.IndexOf(filter2count.Item1) + filter2count.Item1.Length);
				cycles = cycles.Substring(0,cycles.IndexOf(filter2count.Item2));
			}
			catch(Exception){
				return null;
			}
			
			
			List<ShallowHuman> contacts = new List<ShallowHuman>();
			ShallowHuman contact;
			
			
			
			for(int i = 0, count = int.Parse(cycles); i < count; i++){
				try{
					html = html.Substring(html.IndexOf(filter_person));
				}
				catch(Exception){				
					continue;
				}
				contact.id				= isolate(filter_id.Item1, filter_id.Item2, ref html);
				
				if(html.Contains(filter_descr.Item1))
					contact.description	= isolate(filter_descr.Item1, filter_descr.Item2, ref html);
				else
					contact.description	= null;
				
				
				if(html.Contains(filter_extra.Item1))
					contact.extra_info	= isolate(filter_extra.Item1, filter_extra.Item2, ref html);
				else
					contact.extra_info	= null;
				contacts.Add(contact);
			}
			
			return contacts;
		}
		
	}
}