Public Class Grid

    Public width As Integer
    Public height As Integer

    Public bubbles As Array

    Public Sub New(width As Integer, height As Integer, radius As Integer, startHeight As Integer)
        Me.width = width
        Me.height = height
        Dim tbubbles(width - 1, height - 1) As Cell
        bubbles = tbubbles
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If y < startHeight Then
                    bubbles(x, y) = New Cell(New Bubble(0, 0, 0, radius), x, y)
                Else
                    bubbles(x, y) = New Cell(Nothing, x, y)
                End If
            Next
        Next
    End Sub

    Public Sub draw(display As VBGame.Display)
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If Not IsNothing(bubbles(x, y).bubble) Then
                    bubbles(x, y).bubble.draw(display)
                End If
            Next
        Next
    End Sub

End Class
