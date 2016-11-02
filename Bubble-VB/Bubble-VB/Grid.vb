Public Class Grid

    Public width As Integer
    Public height As Integer

    Public won As Boolean = False

    Public minPopSize As Integer = 3

    Public bounds As Size

    Public score As Integer

    Public updateList As New List(Of Cell)
    Private rbubbles As Array
    Public radius As Integer

    Public colorsPresent As List(Of Integer)
    Public finalColor As Integer = 5

    Public lost As Boolean = False

    Public Property bubbles As Array
        Get
            Return rbubbles
        End Get
        Set(value As Array)
            calculateExposed()
        End Set
    End Property

    Public exposed As New List(Of Cell)

    Public Function getColorGroup(x As Integer, y As Integer, Optional ByRef group As List(Of Cell) = Nothing)
        If IsNothing(group) Then
            group = New List(Of Cell)
            group.Add(rbubbles(x, y))
        End If
        For Each Cell As Cell In getNeighbors(x, y)
            If Cell.Bubble.color = rbubbles(x, y).Bubble.color AndAlso Not group.Contains(Cell) Then
                group.Add(Cell)
                getColorGroup(Cell.ix, Cell.iy, group)
            End If
        Next
        Return group
    End Function

    Public Function floodSelect(x As Integer, y As Integer, Optional group As List(Of Cell) = Nothing)
        If IsNothing(group) Then
            group = New List(Of Cell)
            group.Add(rbubbles(x, y))
        End If
        For Each Cell As Cell In getNeighbors(x, y)
            If Not group.Contains(Cell) Then
                group.Add(Cell)
                floodSelect(Cell.ix, Cell.iy, group)
            End If
        Next
        Return group
    End Function

    Public Sub calculateExposed()
        Dim maxNeighbors As Integer
        exposed.Clear()
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1


                If rbubbles(x, y).hasBubble Then
                    If rbubbles(x, y).ix = 0 AndAlso rbubbles(x, y).iy = 0 Then
                        maxNeighbors = 2
                    ElseIf rbubbles(x, y).ix = width - 1 AndAlso rbubbles(x, y).iy = 0 Then
                        maxNeighbors = 3
                    ElseIf rbubbles(x, y).ix = 0 Or rbubbles(x, y).ix = width - 1 Then
                        If ((rbubbles(x, y).iy + 2) Mod 2) = 0 Then
                            maxNeighbors = 3
                        Else
                            maxNeighbors = 5
                        End If
                        If rbubbles(x, y).ix = width - 1 Then
                            If maxNeighbors = 5 Then
                                maxNeighbors = 3
                            Else
                                maxNeighbors = 5
                            End If
                        End If
                    ElseIf rbubbles(x, y).iy = 0 Then
                        maxNeighbors = 4
                    Else
                        maxNeighbors = 6
                    End If
                    If getNeighbors(x, y).Count < maxNeighbors Then
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
                    If (ix >= 0 AndAlso ix < width AndAlso iy >= 0 AndAlso iy < height) AndAlso rbubbles(ix, iy).hasBubble Then
                        neighbors.Add(rbubbles(ix, iy))
                    End If
                End If
            Next
        Next
        Return neighbors
    End Function

    Public Function getAbove(x As Integer, y As Integer)
        Dim above As New List(Of Cell)
        For iy As Integer = y - 1 To 0 Step -1
            above.Add(rbubbles(x, iy))
        Next
        Return above
    End Function

    Public maxRows As Integer

    Public Sub New(size As Size, radius As Integer, startHeight As Integer, availableColors As List(Of Integer))
        Me.colorsPresent = availableColors
        Me.radius = radius
        bounds = New Size(size.Width * radius * 2 + radius, size.Height * radius * 2)
        width = size.Width
        height = size.Height
        Dim tbubbles(width - 1, height - 1) As Cell
        rbubbles = tbubbles
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If y < startHeight Then
                    rbubbles(x, y) = New Cell(New Bubble(0, 0, 0, radius, colorsPresent), x * radius * 2, y * radius * 2, x, y, radius)
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

    Public Function getIslands(Optional toCheck As List(Of Cell) = Nothing) As List(Of Cell)
        If IsNothing(toCheck) Then
            toCheck = exposed
        End If
        Dim checked As New List(Of Cell)
        Dim selection As List(Of Cell)
        Dim islands As New List(Of Cell)
        Dim island As Boolean
        For Each Cell As Cell In toCheck
            If Not checked.Contains(Cell) Then
                selection = floodSelect(Cell.ix, Cell.iy)
                island = True
                For Each sCell As Cell In selection
                    If checked.Contains(sCell) Then
                        island = False
                        checked.AddRange(selection)
                        Exit For
                    End If
                    checked.Add(sCell)
                    If sCell.iy = 0 Then
                        island = False
                        Exit For
                    End If
                Next
                If island Then
                    islands.AddRange(selection)
                End If
            End If
        Next
        Return islands
    End Function

    Public Sub checkLose()
        For Each Cell As Cell In exposed
            If Cell.iy = height - 1 Then
                lost = True
            End If
        Next
    End Sub

    Public Sub findColors()
        colorsPresent.Clear()
        For x As Integer = 0 To width - 1
            For y As Integer = 0 To height - 1
                If rbubbles(x, y).hasBubble AndAlso Not colorsPresent.Contains(rbubbles(x, y).Bubble.color) Then
                    colorsPresent.Add(rbubbles(x, y).Bubble.color)
                End If
            Next
        Next
    End Sub

    Public Sub update(Optional checkColors As Boolean = True)
        calculateExposed()
        For Each Cell As Cell In getIslands()
            exposed.Remove(Cell)
            Cell.pop(Me)
        Next
        If checkColors AndAlso exposed.Count > 0 Then
            findColors()
        End If
        checkLose()
    End Sub

    Public Sub addRow()
        For y As Integer = height - 2 To 0 Step -1
            For x As Integer = 0 To width - 1
                rbubbles(x, y + 1).Bubble = rbubbles(x, y).Bubble
                If y = 0 Then
                    rbubbles(x, y).Bubble = New Bubble(0, 0, 0, radius, colorsPresent)
                Else
                    rbubbles(x, y).Bubble = Nothing
                End If
            Next
        Next
        update(False)
        VBGame.Assets.sounds("advance").play()
    End Sub

    Public Sub snapBubble(bubble As Bubble, player As Player)
        Dim unoccupied As List(Of Cell) = getUnoccupied()
        Dim d As Integer
        Dim lowestIndex As Integer = 0
        Dim lowest As Integer = 999999999
        For i As Integer = 0 To unoccupied.Count - 1
            d = Math.Pow(bubble.x - unoccupied(i).x, 2) + Math.Pow(bubble.y - unoccupied(i).y, 2)
            If d < lowest Then
                lowestIndex = i
                lowest = d
            End If
        Next
        unoccupied(lowestIndex).Bubble = bubble
        Dim lcell As Cell = unoccupied(lowestIndex)
        Dim group As List(Of Cell) = getColorGroup(lcell.ix, lcell.iy)
        If group.Count >= minPopSize Then
            player.addToQueue(colorsPresent)
            For Each cell As Cell In group
                cell.pop(Me)
            Next
        End If
        calculateExposed()
    End Sub

End Class
