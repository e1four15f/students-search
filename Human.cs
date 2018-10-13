	struct Group{
		int quantity;					//численность группы
		string name;					//имя группы
		string url;						//адрес группы
	};

	class Human : ICloneable{
		public int Id			{get;}			//ID человека
		public int Age			{get;}
		public int Plausibility {get; set;}		//оценка правдоподобности
		public string Name		{get;}
		public string Surname	{get;}
		public string Nickname	{get;}
		public string Skype		{get;}
		public string Email		{get;}
		public string University{get;}
		public string MoblilePhone{get;}
		public string HomePhone {get;}
		public string[] Sites	{get;}			//сайты, которые нашлись с помощью операторов google
		public Group [] Groups	{get;}			//массив групп с численностью, названием и ссылкой на группу
		public bool 	Gender	{get;}
		public Human [] Friends {get;}			//друзья данного человека. Нужно будет строить связи людей по этому массиву
		public DateTime DateOfGraduation{get;}	//дата выпуска
		public DateTime DateOfBirth{get;}		//дата рождения
		public string[] Other	{get;}
		
		
		
		public Human(){
		/* Cтандартный конструктор
 		 * инициализирует только Id в -1
 		 * это покажет, что объект пустой
 		 * */
			id = -1;
		}
 		
		public Human(int 	id,
 		             int 	age,
 		             string name,
 		             string surname,
 		             string nickname,
 		             string skype,
 		             string email,
 		             string university,
 		             string mobilePhone,
 		             string homePhone,
 		             string[] sites,
 		             Group [] groups,
 		             bool	  gender,
 		             Human [] friends,
 		             DateTime graduation,
 		             DateTime birth,
 		             string[] other){
 		/*	Полный конструктор, со всеми параметрами
 		 *  | отсутствующую информацию передавать в 
 		 *  | конструктор стандартным значением
 		 */
	 		Id = id;
	 		Age = age;
	 		Name	 = name;
	 		Surname	 = surname;
	 		Nickname = nickname;
	 		Email	 = email;
	 		University 	= university;
	 		MobilePhone = mobilePhone;
	 		HomePhone	= homePhone;
	 		Sites 		= sites;
	 		Groups		= groups;
	 		Gender		= gender;
	 		Friends		= friends;
	 		DateOfGraduation = graduation;
	 		DateOfBirth	= birth;
	 		Other		= other;
 		}
		
		public Human(int 	id,
 		             int 	age,
 		             string name,
 		             string surname,
 		             string nickname,
 		             Group [] groups,
 		             bool	  gender,
 		             Human [] friends,
 		             DateTime birth,
 		             string[] other){
 		/*	Укороченный конструктор, с основными параметрами
 		 *  | отсутствующую информацию передавать в 
 		 *  | конструктор стандартным значением
 		 */
	 		Id = id;
	 		Age = age;
	 		Name	 = name;
	 		Surname	 = surname;
	 		Nickname = nickname;
	 		Groups		= groups;
	 		Gender		= gender;
	 		Friends		= friends;
	 		DateOfBirth	= birth;
	 		Other		= other;
 		}
		
		public Human(int 	id,
 		             string name,
 		             string surname,
 		             Group [] groups,
 		             bool	  gender,
 		             DateTime birth,
 		             string[] other){
 		/*	Минимальный конструктор, с критически малым
 		 *  | отсутствующую информацию передавать в 
 		 *  | конструктор стандартным значением
 		 */
	 		Id = id;
	 		Name	 = name;
	 		Surname	 = surname;
	 		Groups		= groups;
	 		Gender		= gender;
	 		DateOfBirth	= birth;
	 		Other		= other;
 		}
		
		
		public override Human Clone(){
			/*Функция клонирования возвращает this*/
			return this;
		}
		
		public void SetDateOfGraduation(DateTime graduation){
			DateOfGraduation = graduation;
		}
		
		public void SetDateOfBirth(DateTime birth){
			DateOfGraduation = birth;
		}
		
		public void SetDates(DateTime graduation,DateTime birth){
			/*Функция позволяет коротенечко задать сразу обе даты*/
			SetDateOfGraduation(graduation);
			SetDateOfBirth(birth);
		}
		
		
		public int CalcPlausibility(){
			return 100;
		}
		

		bool StudentLookUp(/*Name, Surname, age*/);//проверяет наличие данного человека в базе данных. true - есть, false - нет
		bool /*изменяет (Sites)*/ GoogleLookUp(/*Name, Surname, SkypeNickname, Email, MobilePhone,HomePhone*/);//поиск дополнительных мест обитания человека
		bool /*изменяет (groups)*/ GroupLookUp(/*groups*/);
	}
	