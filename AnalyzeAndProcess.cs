﻿/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 27.10.2018
 * Time: 19:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using HumanData;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MinimalisticTelnet;
using Newtonsoft.Json;

namespace AnalyzeAndProcess
{
	abstract class AnalyzeData
	{
		
		public static bool IsPresent(string surname, string name, DateTime bday, DateTime graduation, ref int plausibility){
			//TODO сделать проверку имени и фамилии
			
			if(CheckAge(bday,graduation))
					plausibility += 5;
			
			return true;
		}
		
		public static bool CheckAge(DateTime bday, DateTime graduation){
			if(!bday.Equals(default(DateTime)) && !graduation.Equals(default(DateTime)))
				if(graduation.Year - bday.Year < 20)
					return true;
			return false;
		}
		
	}


	public delegate bool Searcher(string query, string nickname, string user_agent, ref List<string> profiles);
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
	    
		public static string[] MostLikelySites(Human human){
			/*Проверяет наиболее популярные сайты, на предмет существования там аккаунта человека*/
			
			/*функция скачивания страницы пользователя*/
			WebClient client = new WebClient();
				
			/*строка, скачиваемая WebClient*/
			string html;
			
			
			/*дефолтные сайты, для которые проверяются вне гугла*/
			string[] sites = {"https://pikabu.ru/@",
							  "https://twitter.com/",
							  "https://www.instagram.com/",
							  "https://ask.fm/",
							  "https://www.facebook.com/",
							  "https://habr.com/users/"};
			
			
			/*лист с найденной информацией*/
			List<string> profiles = new List<string>();
			
			/*обрабатываем дефолтные сайты*/
			for(int i = 0; i < sites.Length; i++){
				try{
					html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(sites[i] + human.domain)).ToLower();
				}
				catch(Exception e){
					continue;
				}
				
				/*если на странице есть либо никнейм, либо имя, либо фамилия - добавляем в лист в виде:  айди_вк;профиль_дефолтного_сайта*/
				if ( html.Contains(human.domain) || 
				     html.Contains(human.first_name) || 
				     html.Contains(human.last_name)){
					profiles.Add(sites[i] + human.domain);
				}
			}
			return profiles.ToArray();
		}
			
		public static void Progress(int current, int full){
		/*рисует прогресс чего-то в консоли*/
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
		
		public static string[] GetEmails(Human human){
			/*Возвращает, если есть, список возможных имэйлов пользователя*/
			WebClient client = new WebClient();
			
			/*строка, скачиваемая WebClient*/
			string html;
			
			/*дефолтные провайдеры почты*/
			Tuple<string,string>[] providers = {	new Tuple<string,string>("@yandex.ru","mx.yandex.ru"),
													new Tuple<string,string>("@gmail.com","gmail-smtp-in.l.google.com"),
													new Tuple<string,string>("@mail.ru", "mxs.mail.ru"),
													new Tuple<string,string>("@protonmail.com", "mailsec.protonmail.ch"),
													new Tuple<string,string>("@yahoo.com", "mta7.am0.yahoodns.net"),
													new Tuple<string,string>("@rambler.ru","inmx.rambler.ru")};
													
			
			/*лист с найденной информацией по нику*/
			List<string> profiles = new List<string>();
			
			/*проверяем всех провайдеров, и если почта удовлетворяет всем требованиям - заносим в лист*/
			for(int i = 0; i < providers.Length; i++){
				string email = human.domain + providers[i].Item1;
				if(IsValidEmail(email,providers[i].Item2) && human.domain != null && !human.domain.Contains("id"))
					profiles.Add(email);
				
				if(human.skype == null)
					continue;
				
				email = human.skype + providers[i].Item1;
				
				if(human.skype != null && IsValidEmail(email,providers[i].Item2) )
					profiles.Add(email);
			}
			
			return profiles.ToArray();
		}
		
		static bool IsValidEmail(string email,string provider){
			
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
		
		
		/********************************************************************/
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
		
       	
	
		static bool Google	(string query, string nickname, string user_agent, ref List<string> profiles){
        	
            WebClient client = new WebClient();
            client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
				html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();//client.DownloadString(query + nickname).ToLower(); //
			}
			catch(Exception e){
				html = null;
			}
        	
            if(html == null){
            	return false;
            }
            		
			for(int j = 0; j < 20; j++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "/url?q=";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("&amp"));

			foreach(string dnw2contain in ignore){
				if(googled_url.Contains(dnw2contain))
					goto upper_for;
            }
			for(int i = 0; i < sites.Length; i++){
				if (googled_url.Contains(sites[i]))
		                if (!profiles.Contains(googled_url)){
		            	lock(profile_locker)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            
            return true;
		}
		
		static bool Bing	(string query, string nickname, string user_agent, ref List<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
				html =  client.DownloadString(query + nickname).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();
				}
			catch(Exception e){
				html = null;
			}
            
        	if(html == null){
            	return false;
            }
			for(int j = 0; j < 20; j++){
			/*вычленяем в googled_url ссылку из всего html*/
			string get_url = "<a href=\"";
			html = html.Substring(html.IndexOf(get_url)+get_url.Length);
			string googled_url = html.Substring(0,html.IndexOf("&amp"));

			foreach(string dnw2contain in ignore){
				if(googled_url.Contains(dnw2contain))
					goto upper_for;
            }
			for(int i = 0; i < sites.Length; i++){
				if (googled_url.Contains(sites[i]))
		                if (!profiles.Contains(googled_url)){
		            	lock(profile_locker)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            
            return true;
		}
		
        static bool Yandex	(string query, string nickname, string user_agent, ref List<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
            	html =  System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(google + nickname)).ToLower();
			}
			catch(Exception e){
				html = null;
			}
        	if(html == null){
            	return false;
            }
            
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
		                if (!profiles.Contains(googled_url)){
		            	lock(profile_locker)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            return true;
		}
        
        static bool Rambler	(string query, string nickname, string user_agent, ref List<string> profiles){
        	WebClient client = new WebClient();
        	client.Headers.Add ("user-agent", user_agent);
            string html;
			try{
				html =  client.DownloadString(query + nickname).ToLower(); //System.Text.Encoding.UTF8.GetString(client.DownloadData(query + nickname)).ToLower();
			}
			catch(Exception e){
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
		                if (!profiles.Contains(googled_url)){
		            	lock(profile_locker)
		                    profiles.Add(googled_url);
				}
			}
            upper_for:
	            	continue;
            }
            return true;
		}
        
		static string[] SearchInNet(Human human, List<string> user_agents)
        {
            /*функция скачивания страницы пользователя*/
            WebClient client = new WebClient();
            
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
            string user_agent;
            do{
            	dice = new Random().Next(0,3);
            	call_search.TryGetValue(dice,out current_searcher);
            	
            	int random_user_agent = new Random().Next(0,user_agents.Count);
            	/*достаем в user_agent случайный user-agent из листа*/
            	user_agent = user_agents[random_user_agent];
            }while(!current_searcher(search_queries[dice], human.domain, user_agent, ref profiles));

            return profiles.ToArray();
        }
		
	}
}