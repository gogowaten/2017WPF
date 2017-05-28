Imports Microsoft.VisualStudio.DebuggerVisualizers
Imports System.Drawing

<Assembly: System.Diagnostics.DebuggerVisualizer(
    GetType(BitmapVisualizer.DebuggerTest), GetType(VisualizerObjectSource), Target:=GetType(System.Drawing.Bitmap), Description:="testttoooo")>
Public Class DebuggerTest
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        'Throw New NotImplementedException()
        If windowService Is Nothing Then Throw New ArgumentNullException("windowService")
        If objectProvider Is Nothing Then Throw New ArgumentNullException("objectProvider")
        'Dim str As String = objectProvider.GetObject.ToString
        'MsgBox(str)

        Dim bmp As Bitmap = objectProvider.GetObject
        Dim v As New MyViewer()
        'Call TestShowVisualizer(bmp)
        v.SetBitmap(bmp)
        'v.Show()
        'v.ShowDialog()

    End Sub
    'Public Shared Sub TestShowVisualizer(obj As Object)
    '    Dim visualizerHosut As VisualizerDevelopmentHost = New VisualizerDevelopmentHost(obj, GetType(DebuggerTest))
    '    visualizerHosut.ShowVisualizer()
    'End Sub
End Class

