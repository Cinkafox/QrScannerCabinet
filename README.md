## Клонирование проекта: 
Для успешного клонирования проекта необходимо наличие установленной системы управления версиями Git. Воспользуйтесь следующей командой для клонирования репозитория:
```shell
git clone https://github.com/Cinkafox/QrScannerCabinet
cd QrScannerCabinet
```
## Сборка проекта:
Для сборки всего проекта требуется предварительная установка .NET SDK 8. Скачать данный компонент можно по следующей ссылке:
[Скачать .NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.302-windows-x64-installer)   

Кроме того, для сборки клиентской части проекта потребуется установка Android SDK. Руководство по установке можно найти по следующей ссылке:
[Установка Android SDK](https://learn.microsoft.com/ru-ru/previous-versions/xamarin/android/get-started/installation/android-sdk?tabs=windows)

Все последующие команды выполнять из корневой директории проекта.

Для успешной сборки клиентской части дополнительно необходимо установить maui-android. Выполните следующую команду для установки:
```shell
.\Tools\maui-install.cmd
```

После всех вышеуказанных действий, для финальной сборки всего проекта, выполните следующую команду:
```shell
 .\Tools\build.cmd
```
