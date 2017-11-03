'リサイズハンドルをAdornerで実装する -CoMoの日記
'http://d.hatena.ne.jp/CoMo/20110428/1303996288
'うまくいかない

Imports System.Windows.Controls.Primitives


Class MainWindow
    Private Sub ResizeThumb_DragDelta(sender As Object, e As DragDeltaEventArgs)
        Dim t As Thumb = sender
        If t Is Nothing Then Return

        Dim adored As FrameworkElement = AdornedBy.GetAdornedElementFromTemplateChild(t)
        If adored Is Nothing Then Return

        If t.Name = "ResizeThumb_LT" Or t.Name = "ResizeThumb_LB" Then
            Canvas.SetLeft(adored, Canvas.GetLeft(adored) + e.HorizontalChange)
            adored.Width = Math.Max(20, adored.Width - e.HorizontalChange)
        Else
            adored.Width = Math.Max(20, adored.Width + e.HorizontalChange)
        End If

        If t.Name = "ResizeThumb_LT" Or t.Name = "ResizeThumb_RT" Then
            Canvas.SetTop(adored, Canvas.GetTop(adored) + e.VerticalChange)
            adored.Height = Math.Max(20, adored.Height - e.VerticalChange)
        Else
            adored.Height = Math.Max(20, adored.Height + e.VerticalChange)
        End If
        e.Handled = True

    End Sub
End Class

Public Class AdornedBy
    Inherits Adorner
    Private _Content As FrameworkElement

    Private Sub AddToAdornerLayer()
        Dim layer As AdornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement)
        If layer Is Nothing Then
            Throw New InvalidOperationException("XAML tree must have as lest one AdornerDecorator")
        End If
        Dim registed As Adorner() = layer.GetAdorners(AdornedElement)
        If registed IsNot Nothing Then
            For Each ad In registed
                If ad.GetType Is GetType(AdornedBy) Then
                    layer.Remove(ad)
                End If
            Next
        End If
        layer.Add(Me)
    End Sub

    Public Shared Function GetAdornedElementFromTemplateChild(contained As FrameworkElement) As UIElement
        Dim tp As FrameworkElement = contained.TemplatedParent
        If tp Is Nothing Or tp.GetType <> GetType(Control) Then Return Nothing
        Dim aby As AdornedBy = tp.Parent
        If aby Is Nothing Then Return Nothing
        Return aby.AdornedElement
    End Function


    Private Shared Sub OnTemplateChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim adorned As FrameworkElement = d
        Dim ab As AdornedBy = New AdornedBy(adorned)

        If adorned.IsInitialized Then
            ab.AddToAdornerLayer()
        Else
            'ここがわからない
            'AddHandler adorned.Loaded, AddressOf ab.AddToAdornerLayer
            AddHandler adorned.Loaded, Sub()
                                           ab.AddToAdornerLayer()
                                       End Sub

        End If

        Dim t As ControlTemplate = e.NewValue
        Dim ctrl = New Control With {.Template = t}
        With ab
            ._Content = ctrl
            .AddVisualChild(ctrl)
            .AddLogicalChild(ctrl)
            .InvalidateVisual()
        End With
    End Sub

    Public Shared ReadOnly TemplateProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        "Template", GetType(ControlTemplate), GetType(AdornedBy), New PropertyMetadata(Nothing, AddressOf OnTemplateChanged))
    'Public Property MyTemplate As ControlTemplate
    '    Get
    '        Return GetValue(MyTemplateProperty)
    '    End Get
    '    Set(value As ControlTemplate)
    '        SetValue(MyTemplateProperty, value)
    '    End Set
    'End Property
    Public Shared Function GetTemplate(obj As DependencyObject) As ControlTemplate
        Return obj.GetValue(TemplateProperty)
    End Function
    Public Shared Sub SetTemplate(obj As DependencyObject, value As ControlTemplate)
        obj.SetValue(TemplateProperty, value)
    End Sub

    Public Sub New(adorunedElement As UIElement)
        MyBase.New(adorunedElement)
    End Sub


    Protected Overrides Function MeasureOverride(constraint As Size) As Size
        'Return MyBase.MeasureOverride(constraint)
        Return AdornedElement.DesiredSize
    End Function

    Protected Overrides Function ArrangeOverride(finalSize As Size) As Size
        'Return MyBase.ArrangeOverride(finalSize)
        Return AdornedElement.DesiredSize
    End Function

    Protected Overrides ReadOnly Property VisualChildrenCount As Integer
        Get
            'Return MyBase.VisualChildrenCount
            Return 1
        End Get
    End Property

    Protected Overrides Function GetVisualChild(index As Integer) As Visual
        'Return MyBase.GetVisualChild(index)
        Return _Content
    End Function

End Class