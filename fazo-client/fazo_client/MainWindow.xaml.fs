namespace Views

open System
open System.IO
open System.Windows.Input
open System.Windows
open System.Windows.Forms
open System.Reflection
open System.Drawing
open FsXaml
open Gma.System.MouseKeyHook
open Hardcodet.Wpf.TaskbarNotification
open FSharp.Data

type MainView = XAML<"MainWindow.xaml", true>

type ShowMessageCommand() =
    let event = new DelegateEvent<EventHandler>()
    let shutdown() = Application.Current.Shutdown()

    interface ICommand with
        [<CLIEvent>]
        member x.CanExecuteChanged = event.Publish
        member x.CanExecute param = true
        member x.Execute param = shutdown()

type MainWindowViewController() =
    inherit WindowViewController<MainView>()

    override __.OnInitialized window =
        let currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        let filePath = Path.Combine(currentDir, "Resources", "Icon.ico")
        let icon = new Drawing.Icon(filePath)
        let mutable tbi = new TaskbarIcon(Icon=icon, ToolTipText="Fazo", LeftClickCommand=new ShowMessageCommand())

        let capture() =
            let fileName = "fazo_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png"
            use bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
            use bmpGraphics = Graphics.FromImage(bmp)
            bmpGraphics.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), bmp.Size)
            use ms = new MemoryStream()
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            let userName = Environment.UserName
            Http.RequestString("http://localhost:9000", headers = ["userName", userName; "fileName", fileName], body = BinaryUpload(ms.GetBuffer())) |> ignore
            ()
        let keyDown (evArgs:System.Windows.Forms.KeyEventArgs) =
            if evArgs.KeyCode = Keys.E && evArgs.Modifiers = Keys.Control
            then capture()
            else ()
        let  m_GlobalHook = Hook.GlobalEvents()
        m_GlobalHook.KeyDown.Add(keyDown)

        window.Root.Hide()