'Window.Resourcesの、C#での記述が分かりません
'https://social.msdn.microsoft.com/Forums/silverlight/ja-JP/08759383-00cc-4891-bf05-7755ad9e01dc/windowresourcesc?forum=wpfja


Public Class Window1

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        wrapPanel.Children.Add(New Button With {.Style = Me.Resources("AddButtonStyle"),
                               .Width = 40, .Height = 100, .Content = "Forcusde"})
    End Sub
End Class
