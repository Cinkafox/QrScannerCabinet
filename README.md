## Клонирование проекта:
Для клонирования проекта нужен git.
```shell
git clone https://github.com/Cinkafox/QrScannerCabinet
```

## Сборка проекта:
Для сборки всего проекта вам понадовится скачать .NET SDK 8:
https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.302-windows-x64-installer

Так же, для сборки клиента понадовится Android SDK:
https://learn.microsoft.com/ru-ru/previous-versions/xamarin/android/get-started/installation/android-sdk?tabs=windows

Все команды выполнять в корневой папке проекта.

Для успешной сборки клиенсткой части вам так же понадовится maui-android
```shell
.\Tools\maui-install.cmd
```

В конце, собираем весь проект.
```shell
.\Tools\build.cmd      
```