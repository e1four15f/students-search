# Утилиты

GeneralFunctions.cs
------
Файл содержит стандартное (я везде его использовал), отлаженное тело главных функций утилит:
Write (запись в файл), ThreadProc (функция-агрегатор), Progress(вывод шкалы прогресса в консоль), Main и прототипа Download

Если вдруг в какой-то из утилит что-то не работает на запись/странно выполняется, то функции нужно взять отсюда

MakeUnique.cs
------
Утилита убирает из входного файла вида id;данные повторяющиеся и ошибочные поля, каким-либо образом появившиеся в выходном файле

MostLikelySites
------
Утилита, проверяющая наиболее популярные сайты, на предмет существования там аккаунта пользователя, есть возможность помещать в лог 3 первых валидных ссылки из гугла (нужно убрать коммент с дефайна)

Searcher (НЕ ОТЛАЖЕНА)
------
Утилита делает запросы в поисковики по кастомному айди и сохраняет результаты в файл

SkypeAndEmail
------
Утилита, сохраняющая в файл данные вида 
id;#skype (если есть)
id;email@example.com

Скайп берется со страницы пользователя, имэйлы формируются по нику и скайпу, указанному в профиле, проверяются через телнет на существование. Если почтовый сервер возвращает 250 - абонент существует, 550 - нет (сервер, я думаю, может и обманывать). !ТРЕБУЕТ подключения к проекту TelnetInterface.cs

VerifiedEmails
------
Скачивает имена аккаунтов с форума abiturient.ru если в них содержится @
