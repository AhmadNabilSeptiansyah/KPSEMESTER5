Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Drawing.Printing
Public Class Transaksi
    Dim connectionString As String = "Data Source=MSI; initial catalog=DB_FOOD; Integrated Security=true"
    Public selectedValue As String
    Public stok As Integer
    Dim WithEvents PD As New PrintDocument
    Dim PPD As New PrintPreviewDialog
    Dim longpaper As Integer
    Sub kondisiAwal()
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using sda As New SqlDataAdapter("select Tbl_Transaksi.id_Transaksi,Tbl_barang.kode_barang,Tbl_barang.nama_barang,Tbl_barang.harga_satuan,Tbl_Transaksi.qty,Tbl_Transaksi.total_bayar,Tbl_Transaksi.tgl_transaksi from Tbl_Transaksi inner join Tbl_barang on Tbl_Transaksi.kode_barang=Tbl_barang.kode_barang", Conn)
                Using ds As New DataSet
                    sda.Fill(ds, "Tbl_Transaksi")
                    DataGridView1.DataSource = (ds.Tables("Tbl_Transaksi"))
                    DataGridView1.Columns(0).HeaderText = "ID Transaksi"
                End Using
            End Using
            Conn.Close()
        End Using
        cmbMenu.Enabled = False
        txtQty.Enabled = False
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
    Sub kosongkanData()
        txtId.Text = ""
        txtHarga.Text = ""
        txtQty.Text = ""
        txtTotal.Text = ""
        cmbMenu.Text = ""
    End Sub
    Sub txtEnabled()
        cmbMenu.Enabled = True
        txtQty.Enabled = True
    End Sub

    Private Sub cekStok()
        Dim kd As String = selectedValue.Split("-")(0)
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Using cmd As New SqlCommand("select jumlah_barang from Tbl_barang where kode_barang=@kode", conn)
                cmd.Parameters.AddWithValue("@kode", kd)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                If sdr.Read() Then
                    stok = Convert.ToInt32(sdr("jumlah_barang"))
                End If
                sdr.Close()
            End Using
            conn.Close()
        End Using
    End Sub
    Private Sub updateStok()
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Dim kd As String = selectedValue.Split("-")(0)
            Using cmd As New SqlCommand("update Tbl_barang set jumlah_barang=jumlah_barang - @jumlahbeli where kode_barang=@kode", Conn)
                cmd.Parameters.AddWithValue("@jumlahbeli", txtQty.Text)
                cmd.Parameters.AddWithValue("@kode", kd)
                cmd.ExecuteNonQuery()
            End Using
            Conn.Close()
        End Using
    End Sub

    Private Sub btnTambah_Click(sender As Object, e As EventArgs) Handles btnTambah.Click
        If btnTambah.Text = "Tambah" Then
            btnTambah.Text = "Simpan"
            kosongkanData()
            txtEnabled()
        Else
            Dim kd As String = selectedValue.Split("-")(0)
            cekStok()
            Using Conn As New SqlConnection(connectionString)
                Dim countTr As Integer = 0
                Dim totalBayar As Double
                Conn.Open()
                Using cmd As New SqlCommand("select COUNT(*) from Tbl_Transaksi where kode_barang = @kd", Conn)
                    cmd.Parameters.AddWithValue("@kd", kd)
                    countTr = CInt(cmd.ExecuteScalar)
                End Using
                Using cmd As New SqlCommand("select total_bayar from Tbl_Transaksi where kode_barang = @kd", Conn)
                    cmd.Parameters.AddWithValue("@kd", kd)
                    totalBayar = CDbl(cmd.ExecuteScalar())
                End Using
                If stok < Convert.ToInt32(txtQty.Text) Then
                    MsgBox("stok barang tersisi " & stok)
                    kosongkanData()
                ElseIf stok >= Convert.ToInt32(txtQty.Text) Then
                    If countTr > 0 Then
                        Using cmd As New SqlCommand("update Tbl_Transaksi set qty=qty+@qty,total_bayar= @subtotal where kode_barang=@kd", Conn)
                            If Not String.IsNullOrEmpty(txtTotal.Text) Then
                                cmd.Parameters.AddWithValue("@qty", txtQty.Text)
                                cmd.Parameters.AddWithValue("@subtotal", FormatCurrency(totalBayar + CDbl(txtTotal.Text)))
                                cmd.Parameters.AddWithValue("@kd", kd)
                                cmd.ExecuteNonQuery()
                                updateStok()
                                kondisiAwal()
                                btnTambah.Text = "Tambah"
                                kosongkanData()
                                rumusTotal()
                            Else
                                MsgBox("Total tidak valid")
                                kosongkanData()
                            End If
                        End Using
                    Else
                        Using cmd As New SqlCommand("insert into Tbl_Transaksi (no_transaksi,tgl_transaksi,total_bayar,qty,kode_barang) values (@notran,@tgl,@subtotal,@qty,@kd)", Conn)
                            cmd.Parameters.AddWithValue("@qty", txtQty.Text)
                            cmd.Parameters.AddWithValue("@subtotal", txtTotal.Text)
                            cmd.Parameters.AddWithValue("@tgl", DateTime.Now())
                            cmd.Parameters.AddWithValue("@notran", DateTime.Now())
                            cmd.Parameters.AddWithValue("@kd", kd)
                            cmd.ExecuteNonQuery()
                            updateStok()
                            kondisiAwal()
                            btnTambah.Text = "Tambah"
                            kosongkanData()
                            rumusTotal()
                        End Using
                    End If
                End If
                Conn.Close()
            End Using
        End If
    End Sub

    Sub cmbdata()
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_barang", conn)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                cmbMenu.Items.Clear()
                While sdr.Read()
                    cmbMenu.Items.Add(sdr("kode_barang") & "-" & sdr("nama_barang"))
                End While
                sdr.Close()
            End Using
            conn.Close()
        End Using
    End Sub
    Sub rumusTotal()
        Dim total As Decimal
        For Each row As DataGridViewRow In DataGridView1.Rows

            total += CDbl(row.Cells("total_bayar").Value)
        Next
        labelharga.Text = FormatCurrency(total)
    End Sub
    Private Sub Transaksi_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kondisiAwal()
        txtHarga.Enabled = False
        txtTotal.Enabled = False
        cmbdata()
        'rumusTotal()
    End Sub


    Private Sub cmbMenu_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMenu.SelectedIndexChanged
        selectedValue = cmbMenu.SelectedItem
        Dim kd As String = selectedValue.Split("-")(0)
        Dim nama As String = selectedValue.Split("-")(1)

        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_barang where kode_barang=@id and nama_barang=@nama", Conn)
                cmd.Parameters.AddWithValue("@id", kd)
                cmd.Parameters.AddWithValue("@nama", nama)
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                While sdr.Read()
                    txtId.Text = sdr("kode_barang")
                    txtHarga.Text = sdr("harga_satuan")
                End While
                sdr.Close()
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub txtQty_TextChanged(sender As Object, e As EventArgs) Handles txtQty.TextChanged
        Dim total, harga As Double
        If Decimal.TryParse(txtHarga.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, total) Then
            'harga = Decimal.Parse(txtHarga.Text.Replace("Rp", "").Replace(".", "").Replace(",00", ""))
            harga = CDbl(txtHarga.Text)
            total = harga * Val(txtQty.Text)
            txtTotal.Text = FormatCurrency(total)
        End If
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        LogActivity("Logout")
        Me.Visible = False
        Login.Visible = True
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_Laporan (id_transaksi,nama_barang,harga,qty,total_bayar,tgl_transaksi) values (@id,@nama,@harga,@qty,@subtotal,@tgl)", Conn)
                For Each baris As DataGridViewRow In DataGridView1.Rows
                    If Not baris.IsNewRow Then
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(baris.Cells(0).Value))
                        cmd.Parameters.AddWithValue("@nama", Convert.ToString(baris.Cells(2).Value))
                        cmd.Parameters.AddWithValue("@harga", Convert.ToString(baris.Cells(3).Value))
                        cmd.Parameters.AddWithValue("@qty", Convert.ToInt32(baris.Cells(4).Value))
                        cmd.Parameters.AddWithValue("@subtotal", Convert.ToString(baris.Cells(5).Value))
                        cmd.Parameters.AddWithValue("@tgl", Convert.ToDateTime(baris.Cells(6).Value))
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                        labelharga.ResetText()
                        labelKembali.ResetText()
                        TextBox1.Clear()
                    End If
                Next
            End Using
            Using cmd As New SqlCommand("delete from Tbl_Transaksi", Conn)
                cmd.ExecuteNonQuery()
                kondisiAwal()
            End Using
            Conn.Close()
        End Using
        btnTambah.Text = "Tambah"
    End Sub
    Private Sub Transaksi_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        cmbdata()
        Using Conn As New SqlConnection(connectionString)
            Conn.Open()
            Using cmd As New SqlCommand("insert into Tbl_Laporan (id_transaksi,nama_barang,harga,qty,total_bayar,tgl_transaksi) values (@id,@nama,@harga,@qty,@subtotal,@tgl)", Conn)
                For Each baris As DataGridViewRow In DataGridView1.Rows
                    If Not baris.IsNewRow Then
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(baris.Cells(0).Value))
                        cmd.Parameters.AddWithValue("@nama", Convert.ToString(baris.Cells(2).Value))
                        cmd.Parameters.AddWithValue("@harga", Convert.ToString(baris.Cells(3).Value))
                        cmd.Parameters.AddWithValue("@qty", Convert.ToInt32(baris.Cells(4).Value))
                        cmd.Parameters.AddWithValue("@subtotal", Convert.ToString(baris.Cells(5).Value))
                        cmd.Parameters.AddWithValue("@tgl", Convert.ToDateTime(baris.Cells(6).Value))
                        cmd.ExecuteNonQuery()
                        kondisiAwal()
                    End If
                Next
                'cmd.Parameters.AddWithValue("@id")
            End Using
            Using cmd As New SqlCommand("delete from Tbl_Transaksi", Conn)
                cmd.ExecuteNonQuery()
                kondisiAwal()
                TextBox1.Text = ""
                labelharga.Text = ""
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text >= CDbl(labelharga.Text) Then
            Dim total As Decimal
            total = CDbl(TextBox1.Text) - CDbl(labelharga.Text)
            labelKembali.Text = FormatCurrency(total)
        Else
            MsgBox("Nominal Pembayaran Kurang")
            TextBox1.Text = ""
        End If
    End Sub


    Sub changelongpaper()
        Dim rowcount As Integer
        longpaper = 0
        rowcount = DataGridView1.Rows.Count
        longpaper = rowcount * 15
        longpaper = longpaper + 240
    End Sub
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        changelongpaper()
        PPD.Document = PD
        PPD.ShowDialog()
    End Sub

    Private Sub PD_BeginPrint(sender As Object, e As PrintEventArgs) Handles PD.BeginPrint
        'Dim pagesetup As New PageSettings
        'pagesetup.PaperSize = New PaperSize("Custom", 300, 500) 'fixed size
        Dim paperSize As New PaperSize("Custom", 300, 500)
        'pagesetup.PaperSize = New PaperSize("Custom", 250, longpaper)
        PD.DefaultPageSettings.PaperSize = paperSize
    End Sub

    Private Sub PD_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PD.PrintPage
        Dim f10 As New Font("Times New Roman", 10, FontStyle.Regular)
        Dim f10b As New Font("Times New Roman", 10, FontStyle.Bold)
        Dim f14 As New Font("Times New Roman", 14, FontStyle.Bold)

        Dim leftmargin As Integer = PD.DefaultPageSettings.Margins.Left
        Dim centermargin As Integer = PD.DefaultPageSettings.PaperSize.Width / 2
        Dim rightmargin As Integer = PD.DefaultPageSettings.PaperSize.Width

        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center

        Dim garis As String
        garis = "------------------------------------------------------------------"

        e.Graphics.DrawString("Food XYZ", f14, Brushes.Black, centermargin, 5, tengah)
        e.Graphics.DrawString("JL.Raya Mangun Jaya, Tambun Selatan, Bekasi," & vbNewLine & " Kab Bekasi, Jawa Barat, 17510", f10, Brushes.Black, centermargin, 30, tengah)
        e.Graphics.DrawString("Hp: 0858-1743-3974", f10, Brushes.Black, centermargin, 65, tengah)

        e.Graphics.DrawString("Nama Kasir :", f10, Brushes.Black, 0, 90)
        e.Graphics.DrawString("faric", f10, Brushes.Black, 75, 90)

        e.Graphics.DrawString(Date.Now(), f10, Brushes.Black, 0, 105)
        e.Graphics.DrawString("Nama", f10, Brushes.Black, 0, 125)
        e.Graphics.DrawString("qty", f10, Brushes.Black, 50, 125)
        e.Graphics.DrawString("harga", f10, Brushes.Black, 80, 125)
        e.Graphics.DrawString("Total", f10, Brushes.Black, rightmargin, 125, kanan)
        e.Graphics.DrawString(garis, f10, Brushes.Black, 0, 130)
        Dim tinggi As Integer
        Dim total As Integer
        For Each baris As DataGridViewRow In DataGridView1.Rows
            If Not baris.IsNewRow Then
                tinggi += 15
                e.Graphics.DrawString(baris.Cells(2).Value, f10, Brushes.Black, 0, 130 + tinggi)
                e.Graphics.DrawString(baris.Cells(4).Value, f10, Brushes.Black, 50, 130 + tinggi)
                e.Graphics.DrawString(baris.Cells(3).Value, f10, Brushes.Black, 80, 130 + tinggi)

                e.Graphics.DrawString(baris.Cells(5).Value, f10, Brushes.Black, rightmargin, 130 + tinggi, kanan)
                total += CDbl(baris.Cells(5).Value)
            End If
        Next
        tinggi = 140 + tinggi
        e.Graphics.DrawString(garis, f10, Brushes.Black, 0, tinggi)
        e.Graphics.DrawString("Subtotal :" & FormatCurrency(total), f10b, Brushes.Black, 150, 15 + tinggi)
    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Enter) Then
            TextBox1.Text = FormatCurrency(TextBox1.Text)
        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If btnTambah.Text = "Simpan" Then
            kosongkanData()
            kondisiAwal()
            btnTambah.Text = "Tambah"
        End If
    End Sub
End Class