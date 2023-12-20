Imports System.Data.SqlClient
Imports System.Windows.Forms.DataVisualization.Charting
Public Class kelolaLaporan
    Dim connectionString As String = "Data Source=MSI; initial catalog=DB_FOOD; Integrated Security=true"
    Sub kondisiAwal()
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using sda As New SqlDataAdapter("select id_transaksi,Tgl_Transaksi,subtotal from Tbl_laporan", Conn)
                Using ds As New DataSet
                    sda.Fill(ds, "Tbl_laporan")
                    DataGridView1.DataSource = (ds.Tables("Tbl_laporan"))
                    Chart1.DataSource = ds.Tables("Tbl_laporan")
                    DataGridView1.Columns(0).HeaderText = "ID Transaksi"
                    DataGridView1.Columns(1).HeaderText = "Tanggal Transaksi"
                    DataGridView1.Columns(2).HeaderText = "Total Pembayaran"
                End Using
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub LogActivity(aktifitas As String)
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_Log (waktu,aktivitas,id_User) values (@waktu,@aktifitas,@id)", Conn)
                cmd.Parameters.AddWithValue("@waktu", DateTime.Now())
                cmd.Parameters.AddWithValue("@aktifitas", aktifitas)
                cmd.Parameters.AddWithValue("id", Module1.User)
                cmd.ExecuteNonQuery()
            End Using
            Conn.Close()
        End Using
    End Sub

    Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        Using Conn As New SqlConnection(connectionString)
            Using sda As New SqlDataAdapter("select id_transaksi,tgl_transaksi,total_bayar from Tbl_laporan where(tgl_transaksi BETWEEN '" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "' And '" & Format(DateTimePicker2.Value, "yyyy-MM-dd") & "')", Conn)
                Using ds As New DataSet
                    sda.Fill(ds, "Tbl_Transaksi")
                    DataGridView1.DataSource = (ds.Tables("Tbl_Transaksi"))
                End Using
            End Using
        End Using
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("select tgl_transaksi,total_bayar from Tbl_laporan where(tgl_transaksi BETWEEN '" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "' AND '" & Format(DateTimePicker2.Value, "yyyy-MM-dd") & "')", Conn)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                Chart1.Series("Omset").Points.Clear()
                If sdr.HasRows Then
                    While sdr.Read()
                        Me.Chart1.Series("Omset").XValueType = ChartValueType.Date
                        Me.Chart1.Series("Omset").Points.AddXY(sdr("tgl_transaksi"), sdr("total_bayar"))
                    End While
                Else
                    While sdr.Read()
                        Me.Chart1.Series("Omset").XValueType = ChartValueType.Date
                        Me.Chart1.Series("Omset").Points.AddXY(sdr("Tgl_Transaksi"), 0)
                    End While
                End If
                sdr.Close()
            End Using
        End Using
    End Sub

    Private Sub btnLog_Click(sender As Object, e As EventArgs) Handles btnLog.Click
        Me.Visible = False
        Dasboard.Visible = True
    End Sub

    Private Sub btnKelolaUser_Click(sender As Object, e As EventArgs) Handles btnKelolaUser.Click
        Me.Visible = False
        kelolaUser.Visible = True
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        Me.Visible = False
        Login.Visible = True
        LogActivity("Logout")
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub kelolaLaporan_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class