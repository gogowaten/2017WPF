Imports Microsoft.VisualStudio.DebuggerVisualizers
Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports System.Windows.Media.Imaging
Imports ClassLibrary1_NetFrameworkVisualizer

<Assembly: DebuggerVisualizer(
    GetType(Class2),
    GetType(BitmapSourceImageVisualizerObjectSource),
    Target:=(GetType(BitmapSource)),
    Description:="MyClass2")>
Public Class Class2
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

    Public Shared Sub Testooo(obj As Object)
        Dim vHost As New VisualizerDevelopmentHost(obj, GetType(Class2))
        vHost.ShowVisualizer()
    End Sub

End Class
'Partial Class BitmapSourceImageVisualizerObjectSource
Class BitmapSourceImageVisualizerObjectSource
    Inherits VisualizerObjectSource
    Public Overrides Sub GetData(target As Object, outgoingData As Stream)
        MyBase.GetData(target, outgoingData)

        Dim source As BitmapSource = target
        Dim encoder As New JpegBitmapEncoder With {.QualityLevel = 80}
        Try
            encoder.Frames.Add(BitmapFrame.Create(source))
        Catch ex As Exception
            Throw New Exception("うまくできんかったわ")
        End Try
        Dim outgoingBytes() As Byte
        Using stream = New MemoryStream
            encoder.Save(stream)
            outgoingBytes = stream.ToArray
            stream.Close()
        End Using
        outgoingData.Write(outgoingBytes, 0, outgoingBytes.Length)

    End Sub
End Class
