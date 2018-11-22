/*
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
namespace RuntimePlugin_ns
{
	class RuntimePlugin
	{
		/*	В импортируемом dll плагине должен содержаться класс, содержащий
		 	имя подключаемой dll, и функция, так же содержащая имя dll.
		 	Если требуется вызывать цепочку функций, то это должна быть функция-обертка над остальными.
		 	Если класса или функции, содержащей имя длл не будет, то модуль выкинет ошибку.
		 	Поиск класса и функции не зависит от регистра их имени.
		 	class ExampleDLL, public void ExAmPlEDLL(List<Human> human),при скомпилированном Example.dll будут найдены.
		 	Функция-обертка должна принимать List<Human>, ничего больше*/
		
		Assembly dll;
		string dll_name;
		
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
using HumanData;

namespace ChangeMe_Namespace
{
	public class ChangeMe_ClassName
	{
		/*	В импортируемом dll плагине должен содержаться класс, содержащий
		 	имя подключаемой dll, и функция, так же содержащая имя dll.
		 	Если требуется вызывать цепочку функций, то это должна быть функция-обертка над остальными.
		 	Если класса или функции, содержащей имя длл не будет, то модуль выкинет ошибку.
		 	Поиск класса и функции не зависит от регистра их имени.
		 	class ExampleDLL, public void ExAmPlEDLL(List<Human> human),при скомпилированном Example.dll будут найдены.
		 	Функция-обертка должна принимать List<Human>, ничего больше*/
		 	
		public static void NameMeAsDllFile(string serialized_list){
			List<Human> humans = JsonConvert.DeserializeObject<List<Human>>(serialized_list);
			
		}
	}
}";
			
			//System.IO.File.WriteAllText(path + @"\Program.cs", program);
			//System.IO.File.WriteAllText(path + @"\Human.cs", GUI.Properties.Resources.Human_class);
		}
	}
}