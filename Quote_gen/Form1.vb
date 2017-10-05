﻿Imports System.IO
Imports System.Text
Imports System.Math
Imports System.Globalization
Imports System.Threading
Imports Word = Microsoft.Office.Interop.Word
Imports System.Management
Imports Microsoft.Office.Interop.Word

Public Class Form1
    '----------- directory's-----------
    Dim dirpath_Block As String = "N:\Verkoop\Tekst\Quote_text_block\"
    Dim dirpath_Backup As String = "N:\Verkoop\Aanbiedingen\Quote_gen_backup\"
    Dim dirpath_Home_GP As String = "C:\Temp\"

    Public oWord As Word.Application
    ' see https://support.microsoft.com/en-us/help/316383/how-to-automate-word-from-visual-basic--net-to-create-a-new-document
    Private Sub Generate_word_doc()
        ' Dim oWord As Word.Application
        Dim oDoc As Word.Document
        'Dim oTable As Word.Table
        'Dim oPara1, oPara2, oPara3 As Word.Paragraph
        Dim oPara3 As Word.Paragraph
        Dim ufilename As String
        Dim pathname As String
        Dim style1 As String

        '----------- Select Word style -----------------
        style1 = "N:\VERKOOP\Tekst\Quote_text_block\VTK_Fan_Quote.dotm"

        'Start Word and open the document template. 
        oWord = CType(CreateObject("Word.Application"), Word.Application)
        oWord.Visible = True

        If File.Exists(style1) Then
            oDoc = oWord.Documents.Add(style1.Clone)
        Else
            MessageBox.Show("Dam.. Can not find " & style1)
            oDoc = oWord.Documents.Add
        End If

        '---- find ALL checkboxes controls ---
        '---- sort in Alphabetical order -------
        '---- check for checked ----
        '---- then PRINT 
        TextBox5.Clear()
        Dim all_check As New List(Of Control)
        FindControlRecursive(all_check, Me, GetType(System.Windows.Forms.CheckBox))      'Find the controls
        all_check = all_check.OrderBy(Function(x) x.Text).ToList()  'Alphabetical order

        For i = 0 To all_check.Count - 1
            Dim grbx As System.Windows.Forms.CheckBox = CType(all_check(i), System.Windows.Forms.CheckBox)
            If grbx.Checked = True Then
                oPara3 = oDoc.Content.Paragraphs.Add()
                pathname = dirpath_Block & grbx.Text.Substring(0, 4) & ".docx"
                If File.Exists(pathname) Then
                    TextBox5.Text &= "OK, file found " & pathname & vbCrLf
                    oPara3.Range.InsertFile(pathname)
                Else
                    TextBox5.Text &= "File not found " & pathname & vbCrLf
                End If
            End If
        Next

        '============ search and replace in WORD file================
        'Dim myStoryRange As Range '= oWord.ActiveDocument.Content

        Dim find_s As String = ""
        Dim rep_s As String = ""

        Find_rep(Label1.Text, TextBox1.Text)
        Find_rep(Label3.Text, TextBox7.Text)
        Find_rep(Label4.Text, TextBox8.Text)
        Find_rep(Label5.Text, TextBox9.Text)
        Find_rep(Label6.Text, TextBox2.Text)    'Cust name

        Find_rep(Label7.Text, TextBox11.Text)
        Find_rep(Label8.Text, TextBox12.Text)
        Find_rep(Label9.Text, TextBox13.Text)
        Find_rep(Label11.Text, TextBox14.Text)
        Find_rep(Label12.Text, TextBox15.Text)

        Find_rep(Label24.Text, TextBox24.Text)
        Find_rep(Label25.Text, TextBox25.Text)
        Find_rep(Label26.Text, TextBox26.Text)
        Find_rep(Label27.Text, TextBox27.Text)

        Find_rep(Label21.Text, ComboBox1.SelectedText)
        Find_rep(Label22.Text, ComboBox2.SelectedText)
        Find_rep(Label23.Text, ComboBox3.SelectedText)

        '==================== backup final product===============
        ufilename = "Quote_" & TextBox1.Text & "_" & TextBox2.Text & DateTime.Now.ToString("_yyyy_MM_dd") & ".docx"

        If Directory.Exists(dirpath_Backup) Then
            ufilename = dirpath_Backup & ufilename
        Else
            ufilename = dirpath_Home_GP & ufilename
        End If
        'oWord.ActiveDocument.SaveAs(ufilename.ToString)
    End Sub

    Private Sub Find_rep(find_s As String, rep_s As String)
        '============ search and replace in WORD file================
        Dim myStoryRange As Range

        For Each myStoryRange In oWord.ActiveDocument.StoryRanges
            With myStoryRange.Find
                .Text = find_s.ToString
                .Replacement.Text = rep_s.ToString
                .Wrap = WdFindWrap.wdFindContinue
                .Execute(Replace:=Word.WdReplace.wdReplaceAll)
            End With
            Do While Not (myStoryRange.NextStoryRange Is Nothing)
                myStoryRange = myStoryRange.NextStoryRange
                With myStoryRange.Find
                    .Text = find_s.ToString
                    .Replacement.Text = rep_s.ToString
                    .Wrap = WdFindWrap.wdFindContinue
                    .Execute(Replace:=Word.WdReplace.wdReplaceAll)
                End With
            Loop
        Next myStoryRange
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Check_directories()
        Generate_word_doc()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox3.Text =
        "File naming convention" & vbCrLf & vbCrLf &
        "Text block location is " & vbTab & dirpath_Block.ToString & vbCrLf &
        "Quote backup location is " & vbTab & dirpath_Backup.ToString & vbCrLf &
        "  " & vbCrLf &
        "File-name ate the first 4 character of the checkbox name" & vbCrLf &
        "Printing squence is determined by the file_name sorted in alphabetical order" & vbCrLf &
        " "
        TextBox6.Text =
        "Quotes use font Khmer UI size 10" & vbCrLf &
        "New quotes use the local normal.dot with location" & vbCrLf &
        "C:\\users\(your user name)\appdata\roaming\microsoft\templates.." & vbCrLf
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox1.Text.Trim.Length > 0 And TextBox2.Text.Trim.Length > 0 Then
            Save_tofile()
        Else
            MessageBox.Show("Complete Quote and item number")
        End If
    End Sub
    'Save control settings and case_x_conditions to file
    Private Sub Save_tofile()

        Dim temp_string, user As String

        user = Trim(Environment.UserName)         'User name on the screen
        Dim filename As String = "Quote_select_" & TextBox1.Text & "_" & TextBox2.Text & DateTime.Now.ToString("_yyyy_MM_dd_") & user & ".vtkq"
        Dim all_num, all_combo, all_check, all_text As New List(Of Control)
        Dim i As Integer

        If String.IsNullOrEmpty(TextBox2.Text) Then
            TextBox2.Text = "name"
        End If

        temp_string = TextBox1.Text & ";" & TextBox2.Text & ";"
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"


        '-------- find all checkbox controls and save -------
        FindControlRecursive(all_check, Me, GetType(System.Windows.Forms.CheckBox))      'Find the control
        all_check = all_check.OrderBy(Function(x) x.Name).ToList()  'Alphabetical order
        For i = 0 To all_check.Count - 1
            Dim grbx As System.Windows.Forms.CheckBox = CType(all_check(i), System.Windows.Forms.CheckBox)
            temp_string &= grbx.Checked.ToString & ";"
        Next
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"

        '-------- find all textbox controls and save ----------
        FindControlRecursive(all_text, Me, GetType(System.Windows.Forms.TextBox))      'Find the control
        all_text = all_text.OrderBy(Function(x) x.Name).ToList()  'Alphabetical order
        For i = 0 To all_text.Count - 1
            Dim grbx As System.Windows.Forms.TextBox = CType(all_text(i), System.Windows.Forms.TextBox)
            temp_string &= grbx.Text.ToString & ";"
        Next
        temp_string &= vbCrLf & "BREAK" & vbCrLf & ";"

        Try
            Check_directories()  'Are the directories present
            If CInt(temp_string.Length.ToString) > 5 Then      'String may be empty
                If Directory.Exists(dirpath_Backup) Then
                    File.WriteAllText(dirpath_Backup & filename, temp_string, Encoding.ASCII)      'used at VTK
                Else
                    File.WriteAllText(dirpath_Home_GP & filename, temp_string, Encoding.ASCII)     'used at home
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Line 5062, " & ex.Message)  ' Show the exception's message.
        End Try
    End Sub
    Private Sub Check_directories()
        '---- if path not exist then create one----------
        Try
            If (Not System.IO.Directory.Exists(dirpath_Home_GP)) Then System.IO.Directory.CreateDirectory(dirpath_Home_GP)
            If (Not System.IO.Directory.Exists(dirpath_Block)) Then System.IO.Directory.CreateDirectory(dirpath_Block)
            If (Not System.IO.Directory.Exists(dirpath_Backup)) Then System.IO.Directory.CreateDirectory(dirpath_Backup)
        Catch ex As Exception
        End Try
    End Sub

    'Retrieve control settings and case_x_conditions from file
    'Split the file string into 5 separate strings
    'Each string represents a control type (combobox, checkbox,..)
    'Then split up the secton string into part to read into the parameters
    Private Sub Read_file()
        Dim control_words(), words() As String
        Dim i As Integer
        Dim k As Integer = 0
        Dim all_num, all_combo, all_check, all_text As New List(Of Control)
        Dim separators() As String = {";"}
        Dim separators1() As String = {"BREAK"}

        OpenFileDialog1.FileName = "Quote_select_*"

        If Directory.Exists(dirpath_Backup) Then
            OpenFileDialog1.InitialDirectory = dirpath_Backup  'used at VTK
        Else
            OpenFileDialog1.InitialDirectory = dirpath_Home_GP  'used at home
        End If

        OpenFileDialog1.Title = "Open a Text File"
        OpenFileDialog1.Filter = "VTKQ Files|*.vtkq|VTKQ file|*.vtkq"
        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim readText As String = File.ReadAllText(OpenFileDialog1.FileName, Encoding.ASCII)

            control_words = readText.Split(separators1, StringSplitOptions.None) 'Split the read file content

            '----- retrieve case condition-----
            words = control_words(0).Split(separators, StringSplitOptions.None) 'Split the read file content
            TextBox1.Text = words(0)                  'Project number
            TextBox2.Text = words(1)                  'Item no

            '---------- terugzetten checkbox controls -----------------
            FindControlRecursive(all_check, Me, GetType(System.Windows.Forms.CheckBox))      'Find the control
            all_check = all_check.OrderBy(Function(x) x.Name).ToList()                  'Alphabetical order
            words = control_words(1).Split(separators, StringSplitOptions.None) 'Split the read file content
            For i = 0 To all_check.Count - 1
                Dim grbx As System.Windows.Forms.CheckBox = CType(all_check(i), System.Windows.Forms.CheckBox)
                '--- dit deel voorkomt problemen bij het uitbreiden van het aantal checkboxes--
                If (i < words.Length - 1) Then
                    Boolean.TryParse(words(i + 1), grbx.Checked)
                Else
                    MessageBox.Show("Warning last checkbox not found in file")
                End If
            Next

            '---------- terugzetten textbox controls -----------------
            FindControlRecursive(all_text, Me, GetType(System.Windows.Forms.TextBox))      'Find the control
            all_text = all_text.OrderBy(Function(x) x.Name).ToList()                  'Alphabetical order
            words = control_words(2).Split(separators, StringSplitOptions.None) 'Split the read file content
            For i = 0 To all_text.Count - 1
                Dim grbx As System.Windows.Forms.TextBox = CType(all_text(i), System.Windows.Forms.TextBox)
                '--- dit deel voorkomt problemen bij het uitbreiden van het aantal checkboxes--
                If (i < words.Length - 1) Then
                    grbx.Text = words(i + 1)
                Else
                    MessageBox.Show("Warning last textbox not found in file")
                End If
            Next
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Read_file()
    End Sub

    '----------- Find all controls on form1------
    'Nota Bene, sequence of found control may be differen, List sort is required
    Public Shared Function FindControlRecursive(ByVal list As List(Of Control), ByVal parent As Control, ByVal ctrlType As System.Type) As List(Of Control)
        If parent Is Nothing Then Return list

        If parent.GetType Is ctrlType Then
            list.Add(parent)
        End If
        For Each child As Control In parent.Controls
            FindControlRecursive(list, child, ctrlType)
        Next
        Return list
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click, Button4.Enter, TabPage10.Enter
        Dim i As Integer
        Dim k As Integer = 0
        Dim all_check As New List(Of Control)
        Dim separators() As String = {";"}
        Dim separators1() As String = {"BREAK"}

        TextBox4.Clear()

        '-------- find all checkbox controls and save
        FindControlRecursive(all_check, Me, GetType(System.Windows.Forms.CheckBox))      'Find the control
        all_check = all_check.OrderBy(Function(x) x.Text).ToList()  'Alphabetical order
        For i = 0 To all_check.Count - 1
            Dim grbx As System.Windows.Forms.CheckBox = CType(all_check(i), System.Windows.Forms.CheckBox)
            If grbx.Checked = True Then
                TextBox4.Text &= grbx.Text & vbCrLf
            End If
        Next
    End Sub

End Class
