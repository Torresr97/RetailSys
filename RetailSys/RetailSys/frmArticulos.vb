Imports System.Data.SqlClient
Imports System.Text
Imports System.Security.Cryptography
Imports System.Drawing.Bitmap

Public Class frmArticulos
    Dim ds As New DataSet

    Dim sqlarticulo = "SELECT idarticulo, articulo, precio,cantidad FROM articulos " &
                        "WHERE UPPER(idarticulo) LIKE UPPER(@idarticulo)"


    Dim daArticulo As New SqlDataAdapter
    Dim frmEstado = 0

    Public Sub New()

        InitializeComponent()


        daArticulo = New SqlDataAdapter(sqlarticulo, My.Settings.cnxDB)
    End Sub

    Private Sub frmUsuarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        daArticulo.SelectCommand.Parameters.AddWithValue("@idarticulo", "%%")

        grdData.AutoGenerateColumns = False
        grdData.Columns(0).DataPropertyName = "idarticulo"
        grdData.Columns(1).DataPropertyName = "articulo"
        grdData.Columns(2).DataPropertyName = "precio"
        grdData.Columns(3).DataPropertyName = "cantidad"

        daArticulo.Fill(ds, "articulos")
        grdData.DataSource = ds.Tables("articulos")

        txtIdarticulo.DataBindings.Add("text", ds.Tables("articulos"), "idarticulo")
        txtarticulo.DataBindings.Add("text", ds.Tables("articulos"), "articulo")
        txtprecio.DataBindings.Add("text", ds.Tables("articulos"), "precio")
        txtcantidad.DataBindings.Add("text", ds.Tables("articulos"), "cantidad")




    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Me.Close()
    End Sub


    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        If frmEstado = 0 Then
            limpiar()
            activar(True)

            txtIdarticulo.Focus()
            btnAgregar.Text = "Guardar"
            btnEditar.Text = "Cancelar"
            btnEliminar.Enabled = False
            btnAgregar.Image = My.Resources.aceptar1
            btnEditar.Image = My.Resources.cancelar1
            frmEstado = 1
        ElseIf frmEstado = 1 Then
            If txtIdarticulo.Text <> "" _
                And txtarticulo.Text <> "" _
                And txtprecio.Text <> "" Then


                Dim strsql = "INSERT INTO articulos(idarticulo, articulo, precio,cantidad) " &
                                    "VALUES(@idarticulo, @articulo, @precio, @cantidad)"

                Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)
                    con.Open()
                    Try

                        Dim cmd As New SqlClient.SqlCommand(strsql, con)
                        cmd.Parameters.AddWithValue("@idarticulo", txtIdarticulo.Text)
                        cmd.Parameters.AddWithValue("@articulo", txtarticulo.Text)
                        cmd.Parameters.AddWithValue("@precio", txtprecio.Text)
                        cmd.Parameters.AddWithValue("@cantidad", txtprecio.Text)




                        cmd.ExecuteNonQuery()
                        limpiar()
                        activar(False)
                        btnAgregar.Text = "Agregar"
                        btnEditar.Text = "Editar"
                        btnEliminar.Enabled = True
                        btnEliminar.Text = "Eliminar"
                        btnAgregar.Image = My.Resources.agregar1
                        btnEditar.Image = My.Resources.editar1
                        btnEliminar.Image = My.Resources.eliminar1
                        reFill()
                        frmEstado = 0
                    Catch ex As Exception
                        MsgBox("Error al realizar la consulta: " & ex.Message, MsgBoxStyle.Critical, "Error")
                    End Try
                End Using



            Else
                MsgBox("Debe llenar todos los campos del Formulario")

            End If
        End If

        If frmEstado = 2 Then

            If txtIdarticulo.Text <> "" _
                And txtarticulo.Text <> "" _
                And txtprecio.Text <> "" Then


                Dim strsqlA = "UPDATE articulos SET articulo=@articulo, precio=@precio, cantidad=@cantidad WHERE idarticulo=@idarticulo"


                Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)
                    con.Open()
                    Try

                        Dim cmd As New SqlClient.SqlCommand(strsqlA, con)
                        cmd.Parameters.AddWithValue("@idarticulo", txtIdarticulo.Text)
                        cmd.Parameters.AddWithValue("@articulo", txtarticulo.Text)
                        cmd.Parameters.AddWithValue("@precio", txtprecio.Text) '
                        cmd.Parameters.AddWithValue("@cantidad", txtprecio.Text)


                        cmd.ExecuteNonQuery()
                        limpiar()
                        activar(False)
                        btnAgregar.Text = "Agregar"
                        btnEditar.Text = "Editar"
                        btnEliminar.Enabled = True
                        btnEliminar.Text = "Eliminar"
                        btnAgregar.Image = My.Resources.agregar1
                        btnEditar.Image = My.Resources.editar1
                        btnEliminar.Image = My.Resources.eliminar1
                        reFill()
                        frmEstado = 0
                    Catch ex As Exception
                        MsgBox("Error al realizar la consulta: " & ex.Message, MsgBoxStyle.Critical, "Error")
                    End Try
                End Using




            End If
        End If


        If frmEstado = 3 Then


            Dim strsqlE = "DELETE FROM articulos WHERE idarticulo=@idarticulo"



            Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)
                con.Open()
                Try

                    Dim cmd As New SqlClient.SqlCommand(strsqlE, con)
                    cmd.Parameters.AddWithValue("@idarticulo", txtIdarticulo.Text)

                    cmd.ExecuteNonQuery()
                    limpiar()
                    activar(False)
                    btnAgregar.Text = "Agregar"
                    btnEditar.Text = "Editar"
                    btnEliminar.Enabled = True
                    btnEliminar.Text = "Eliminar"
                    btnAgregar.Image = My.Resources.agregar1
                    btnEditar.Image = My.Resources.editar1
                    btnEliminar.Image = My.Resources.eliminar1
                    reFill()
                    frmEstado = 0
                Catch ex As Exception
                    MsgBox("Error al realizar la consulta: " & ex.Message, MsgBoxStyle.Critical, "Error")
                End Try
            End Using
        End If


    End Sub



    Sub activar(estado As Boolean)
        grdData.Enabled = Not estado

        txtIdarticulo.ReadOnly = Not estado
        txtarticulo.ReadOnly = Not estado
        txtprecio.ReadOnly = Not estado

    End Sub

    Sub limpiar()
        txtIdarticulo.Clear()
        txtarticulo.Clear()
        txtprecio.Clear()

    End Sub

    Sub reFill()
        Try
            grdData.DataSource = Nothing
            txtIdarticulo.DataBindings.Clear()
            txtarticulo.DataBindings.Clear()
            txtprecio.DataBindings.Clear()

            ds.Tables("articulos").Rows.Clear()
            daArticulo.Fill(ds, "articulos")

            grdData.DataSource = ds.Tables("articulos")
            txtIdarticulo.DataBindings.Add("Text", ds.Tables("articulos"), "idarticulo")
            txtarticulo.DataBindings.Add("Text", ds.Tables("articulos"), "articulo")
            txtprecio.DataBindings.Add("Text", ds.Tables("articulos"), "precio")
            txtcantidad.DataBindings.Add("Text", ds.Tables("articulos"), "cantidad")

        Catch ex As Exception

        End Try
    End Sub


    Private Sub btnEditar_Click(sender As Object, e As EventArgs) Handles btnEditar.Click
        If frmEstado = 0 Then
            limpiar()
            activar(True)

            txtIdarticulo.Focus()
            btnAgregar.Text = "Actualizar"
            btnEditar.Text = "Cancelar"
            btnEliminar.Enabled = False
            btnAgregar.Image = My.Resources.aceptar1
            btnEditar.Image = My.Resources.cancelar1
            btnEliminar.Image = My.Resources.eliminar1
            frmEstado = 2

        End If


    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If frmEstado = 0 Then
            limpiar()
            activar(True)
            txtIdarticulo.Focus()
            btnAgregar.Text = "Eliminar"
            btnEditar.Text = "Cancelar"
            btnEliminar.Enabled = False
            btnAgregar.Image = My.Resources.aceptar1
            btnEditar.Image = My.Resources.cancelar1
            btnEliminar.Image = My.Resources.eliminar1

            frmEstado = 3

        End If



    End Sub




End Class