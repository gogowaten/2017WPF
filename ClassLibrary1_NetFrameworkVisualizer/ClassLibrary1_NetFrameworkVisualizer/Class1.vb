Imports Microsoft.VisualStudio.DebuggerVisualizers
Imports System.Drawing
Imports System.Windows.Forms

<Assembly: System.Diagnostics.DebuggerVisualizer(
    GetType(Class1),
    GetType(VisualizerObjectSource),
    Target:=GetType(String),
    Description:="waten")>
Public Class Class1
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Throw New NotImplementedException()
        Dim bmp As Bitmap = objectProvider.GetObject
        Dim form As New Form
        With form
            .Text = String.Format("MyImageView")
            .ClientSize = New Size(480, 320)
            .FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
            .ShowInTaskbar = False
            .ShowIcon = False
        End With

        Dim pb As New PictureBox
        With pb
            .SizeMode = PictureBoxSizeMode.Zoom
            .Image = bmp
            .Parent = form
            .Dock = DockStyle.Fill
        End With

        form.ShowDialog()

    End Sub
End Class
