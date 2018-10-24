/*
 * Created by SharpDevelop.
 * User: VirtualWin
 * Date: 24.10.2018
 * Time: 20:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
		static string[] Download(string id);

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