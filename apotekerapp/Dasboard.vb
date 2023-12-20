Imports System.Data.SqlClient
Public Class Dasboard
    Dim connectionString As String = "Data Source=MSI; initial catalog=DB_FOOD; Integrated Security=true"
    Sub kondisiAwal()
        Using Conn As New SqlConnection(connectionString)
            Using sda As New SqlDataAdapter("select Tbl_Log.id_log,Tbl_Log.waktu,Tbl_Log.aktivitas,Tbl_User.nama from Tbl_Log inner join Tbl_User on Tbl_Log.id_User = Tbl_user.id_User", Conn)
                Dim ds As New DataSet
                sda.Fill(ds, "Tbl_Log")
                DataGridView1.DataSource = ds.Tables("Tbl_Log")
            End Using
        End Using
    End Sub
    Private Sub LogActivity(aktifitas As String)
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_Log (waktu,aktivitas,id_User) values (@waktu,@aktifitas,@id)", Conn)
                cmd.Parameters.AddWithValue("@waktu", DateTime.Now())
                cmd.Parameters.AddWithValue("@aktifitas", aktifitas)
                cmd.Parameters.AddWithValue("@id", Module1.User)
                cmd.ExecuteNonQuery()
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using sda As New SqlDataAdapter("select Tbl_log.id_log,Tbl_user.nama,Tbl_log.waktu,Tbl_log.aktivitas from Tbl_Log inner join Tbl_user on Tbl_log.id_user = Tbl_user.id_user where CONVERT(date,waktu) between '" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "' and  '" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "' ", Conn)
                Dim ds As New DataSet
                sda.Fill(ds, "Tbl_Log")
                DataGridView1.DataSource = (ds.Tables("Tbl_Log"))
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub btnKelolaUser_Click(sender As Object, e As EventArgs) Handles btnKelolaUser.Click, btnLog.Click
        Me.Visible = False
        kelolaUser.Visible = True
    End Sub

    Private Sub btnKelolaLaporan_Click(sender As Object, e As EventArgs) Handles btnKelolaLaporan.Click
        Me.Visible = False
        kelolaLaporan.Visible = True
    End Sub

    Private Sub Dasboard_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        kondisiAwal()
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        LogActivity("Logout")
        Me.Visible = False
        Login.Visible = True
    End Sub

    Private Sub Dasboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btnLog.BackColor = Color.LightYellow
    End Sub
End Class