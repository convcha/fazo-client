namespace ViewModels

open FSharp.ViewModule
open FsXaml

type MainView = XAML<"MainWindow.xaml", true>

type MainViewModel() as self = 
    inherit ViewModelBase()