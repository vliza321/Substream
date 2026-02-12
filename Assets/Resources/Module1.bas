Attribute VB_Name = "Module1"
Option Explicit

Sub saveCSVwithoutBOMbyTableAsCSV()

    Dim strPath, strFileName As String
    Dim i, j, x, y As Integer
    Dim countX, countY
    
    Dim fs As Object 'ADODB.Stream'
    Dim fsText As String
    
    Dim bs As Object 'ADODB.Stream'
            
    strPath = ThisWorkbook.Path & "\"
    ChDir strPath
            
    For i = 1 To ThisWorkbook.Sheets.Count
        ThisWorkbook.Sheets(i).Select
        strFileName = ThisWorkbook.Sheets(i).Name & ".csv"

        countX = 1
        If Trim(Range("B2")) <> "" Then
            countX = Range("A2").End(xlToRight).Column
        End If
        
        countY = 3
        If Trim(Range("A4")) <> "" Then
            countY = Range("A3").End(xlDown).Row
        End If
        
        Set fs = CreateObject("ADODB.Stream")
        fs.Type = 2
        fs.Charset = "utf-8"
        fs.Open
        
        For y = 2 To countY
            For x = 1 To countX
                fsText = Cells(y, x)
                fs.writetext fsText
                If x <> countX Then
                    fs.writetext ","
                End If
            Next
            If y <> countY Then
                fs.writetext vbCrLf
            End If
        Next
        
        fs.Position = 3
        
        Set bs = CreateObject("ADODB.Stream")
        bs.Type = 1
        bs.Open
        
        fs.CopyTo bs
        
        fs.flush
        fs.Close
        
        bs.SaveToFile strPath & strFileName, 2
        bs.flush
        bs.Close
    Next
    
End Sub



