Public Class Grid

    Public width As Integer
    Public height As Integer

    Public bounds As Size

    Private rbubbles As Array

    Public Property bubbles As Array
        Get
            Return rbubbles
        End Get
        Set(value As Array)
            calculateExposed()
        End Set
    End Property

    Public exposed As New List(Of Cell)

    Public Sub calculateExposed()
        exposed.Clear()
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1


                If rbubbles(x, y).hasBubble Then
                    If getNeighbors(x, y).Count <> 6 Then
                        exposed.Add(rbubbles(x, y))
                    End If
                End If

            Next
        Next
    End Sub

    Public Function getNeighbors(x As Integer, y As Integer) As List(Of Cell)
        Dim neighbors As New List(Of Cell)
        Dim isNeighbor As Boolean
        For ix As Integer = x - 1 To x + 1
            For iy As Integer = y - 1 To y + 1
                If ((iy + 2) Mod 2) = 0 Then
                    isNeighbor = Not ((ix - x = -1 And iy - y <> 0) Or (ix = x And iy = y))
                Else
                    isNeighbor = Not ((ix - x = 1 And iy - y <> 0) Or (ix = x And iy = y))
                End If
                If isNeighbor Then
                    Try
                        If rbubbles(ix, iy).hasBubble Then
                            neighbors.Add(rbubbles(ix, iy))
                        End If
                    Catch ex As IndexOutOfRangeException
                    End Try
                End If
            Next
        Next
        Return neighbors
    End Function

    Public maxRows As Integer

    Public Sub New(size As Size, radius As Integer, startHeight As Integer, maxRows As Integer)
        bounds = New Size(size.Width * radius * 2 + radius, size.Height * radius * 2)
        width = size.Width
        height = size.Height
        Dim tbubbles(width - 1, height - 1) As Cell
        rbubbles = tbubbles
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If y < startHeight Then
                    rbubbles(x, y) = New Cell(New Bubble(0, 0, 0, radius), x * radius * 2, y * radius * 2, x, y, radius)
                Else
                    rbubbles(x, y) = New Cell(Nothing, x * radius * 2, y * radius * 2, x, y, radius)
                End If
            Next
        Next
    End Sub

    Public Sub draw(display As Object)
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If Not IsNothing(rbubbles(x, y).bubble) Then
                    rbubbles(x, y).bubble.draw(display)
                End If
            Next
        Next
    End Sub

    Public Function getUnoccupied() As List(Of Cell)
        Dim unoccupied As New List(Of Cell)
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If Not rbubbles(x, y).hasBubble Then
                    unoccupied.Add(rbubbles(x, y))
                End If
            Next
        Next
        Return unoccupied
    End Function

    Public Sub snapBubble(bubble As Bubble)
        Console.WriteLine("Start the machine!")
        Dim unoccupied As List(Of Cell) = getUnoccupied()
        Dim d As Integer
        Dim lowestIndex As Integer = 0
        Dim lowest As Integer = 999999999
        For i As Integer = 0 To unoccupied.Count - 1
            Console.WriteLine(Math.Pow(bubble.x - unoccupied(i).x, 2) + Math.Pow(bubble.y - unoccupied(i).y, 2))
            d = Math.Pow(bubble.x - unoccupied(i).x, 2) + Math.Pow(bubble.y - unoccupied(i).y, 2)
            If d < lowest Then
                lowestIndex = i
                lowest = d
            End If
        Next
        Console.WriteLine("and the lowest goes to: " & lowest & "!")
        unoccupied(lowestIndex).Bubble = bubble
        calculateExposed()
    End Sub

    Public Sub shift(xc As Integer, yc As Integer)
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If Not IsNothing(rbubbles(x, y).bubble) Then
                    rbubbles(x, y).x += xc
                    rbubbles(x, y).y += yc
                End If
            Next
        Next
    End Sub

End Class
