Imports System.Data.SqlClient
Imports System.Text
Imports System.Security.Cryptography
Public Class frmUsuarios
    Dim ds As New DataSet

    Dim sqlUsuarios = "SELECT idusuario, usuario, email FROM usuarios " &
                        "WHERE UPPER(idusuario) LIKE UPPER(@idusuario)"


    Dim daUsuarios As New SqlDataAdapter
    Dim frmEstado = 0 '0=Consulta, 1=Agregando Datos, 2=Editando registro,3=elimando registro

    Public Sub New()
        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        daUsuarios = New SqlDataAdapter(sqlUsuarios, My.Settings.cnxDB)
    End Sub

    Private Sub frmUsuarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        daUsuarios.SelectCommand.Parameters.AddWithValue("@idusuario", "%%")

        grdData.AutoGenerateColumns = False
        grdData.Columns(0).DataPropertyName = "idusuario"
        grdData.Columns(1).DataPropertyName = "usuario"
        grdData.Columns(2).DataPropertyName = "email"

        daUsuarios.Fill(ds, "usuarios")
        grdData.DataSource = ds.Tables("usuarios")

        txtIdUsuario.DataBindings.Add("text", ds.Tables("usuarios"), "idusuario")
        txtUsuario.DataBindings.Add("text", ds.Tables("usuarios"), "usuario")
        txtEmail.DataBindings.Add("text", ds.Tables("usuarios"), "email")

        ' MsgBox( )

    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Me.Close()
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        If frmEstado = 0 Then
            limpiar()
            activar(True)

            txtIdUsuario.Focus()
            btnAgregar.Text = "Guardar"
            btnEditar.Text = "Cancelar"
            btnEliminar.Enabled = False
            btnAgregar.Image = My.Resources.aceptar1
            btnEditar.Image = My.Resources.cancelar1
            frmEstado = 1
        ElseIf frmEstado = 1 Then
            If txtIdUsuario.Text <> "" _
                And txtUsuario.Text <> "" _
                And txtEmail.Text <> "" _
                And txtPassword.Text <> "" And txtConfirmar.Text <> "" Then
                If InStr(txtIdUsuario.Text, " ") = 0 Then
                    If txtPassword.Text = txtConfirmar.Text Then
                        'generamos la llave de forma aleatoria
                        Dim llave As String = GenerateSHA512String(CInt(Int((999999999 * Rnd()) + 1)).ToString)
                        Dim passw As String = GenerateSHA512String(txtPassword.Text)

                        passw = passw & llave
                        passw = GenerateSHA512String(passw)

                        Dim strsql = "INSERT INTO usuarios(idusuario, usuario, email, llave, password) " &
                                    "VALUES(@idusuario, @usuario, @email, @llave, @password)"

                        Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)
                            con.Open()
                            Try
                                'Declaramos el objeto que realiza el query con la conexion abierta.
                                Dim cmd As New SqlClient.SqlCommand(strsql, con)
                                cmd.Parameters.AddWithValue("@idusuario", txtIdUsuario.Text)
                                cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text)
                                cmd.Parameters.AddWithValue("@email", txtEmail.Text)
                                cmd.Parameters.AddWithValue("@password", passw)
                                cmd.Parameters.AddWithValue("@llave", llave)
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
                        MsgBox("La contraseña no coincide.")
                    End If

                Else
                    MsgBox("El codigo del usuario no puede llevar espacios.")
                End If

            Else
                MsgBox("Debe llenar todos los campos del Formulario")

            End If
        End If

        If frmEstado = 2 Then
        If txtIdUsuario.Text <> "" _
                And txtUsuario.Text <> "" _
                And txtEmail.Text <> "" _
                And txtPassword.Text <> "" And txtConfirmar.Text <> "" Then
            If InStr(txtIdUsuario.Text, " ") = 0 Then
                If txtPassword.Text = txtConfirmar.Text Then
                    'generamos la llave de forma aleatoria
                    Dim llave As String = GenerateSHA512String(CInt(Int((999999999 * Rnd()) + 1)).ToString)
                    Dim passw As String = GenerateSHA512String(txtPassword.Text)

                    passw = passw & llave
                    passw = GenerateSHA512String(passw)

                    Dim strsqlA = "UPDATE usuarios SET idusuario=@idusuario, usuario=@usuario, email=@email, llave=@llave, password=@password WHERE idusuario=@idusuario"


                    Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)
                        con.Open()
                        Try
                            'Declaramos el objeto que realiza el query con la conexion abierta.
                            Dim cmd As New SqlClient.SqlCommand(strsqlA, con)
                            cmd.Parameters.AddWithValue("@idusuario", txtIdUsuario.Text)
                            cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text)
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text)
                            cmd.Parameters.AddWithValue("@password", passw)
                            cmd.Parameters.AddWithValue("@llave", llave)
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
                    MsgBox("La contraseña no coincide.")
                End If

            Else
                MsgBox("El codigo del usuario no puede llevar espacios.")
            End If

        Else
            MsgBox("Debe llenar todos los campos del Formulario")

        End If
        End If










        If frmEstado = 3 Then


            Dim strsqlE = "DELETE FROM usuarios WHERE idusuario=@idusuario"



            Using con As New SqlClient.SqlConnection(My.Settings.cnxDB)
                con.Open()
                Try
                    'Declaramos el objeto que realiza el query con la conexion abierta.
                    Dim cmd As New SqlClient.SqlCommand(strsqlE, con)
                    cmd.Parameters.AddWithValue("@idusuario", txtIdUsuario.Text)
                    cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text)
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

        txtIdUsuario.ReadOnly = Not estado
        txtUsuario.ReadOnly = Not estado
        txtEmail.ReadOnly = Not estado
        txtPassword.ReadOnly = Not estado
        txtConfirmar.ReadOnly = Not estado
    End Sub

    Sub limpiar()
        txtIdUsuario.Clear()
        txtUsuario.Clear()
        txtEmail.Clear()
        txtPassword.Clear()
        txtConfirmar.Clear()
    End Sub

    Sub reFill()
        Try
            grdData.DataSource = Nothing
            txtIdUsuario.DataBindings.Clear()
            txtUsuario.DataBindings.Clear()
            txtEmail.DataBindings.Clear()

            ds.Tables("usuarios").Rows.Clear()
            daUsuarios.Fill(ds, "usuarios")

            grdData.DataSource = ds.Tables("usuarios")
            txtIdUsuario.DataBindings.Add("Text", ds.Tables("usuarios"), "idusuario")
            txtUsuario.DataBindings.Add("Text", ds.Tables("usuarios"), "usuario")
            txtEmail.DataBindings.Add("Text", ds.Tables("usuarios"), "email")

        Catch ex As Exception

        End Try
    End Sub

    Function GenerateSHA512String(ByVal inputString) As String
        Dim sha512 As SHA512 = SHA512Managed.Create()
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(inputString)
        Dim hash As Byte() = sha512.ComputeHash(bytes)
        Dim stringBuilder As New StringBuilder()

        For i As Integer = 0 To hash.Length - 1
            stringBuilder.Append(hash(i).ToString("X2"))
        Next

        Return stringBuilder.ToString().ToLower()
    End Function

    Private Sub btnEditar_Click(sender As Object, e As EventArgs) Handles btnEditar.Click
        If frmEstado = 0 Then
            limpiar()
            activar(True)

            txtIdUsuario.Focus()
            btnAgregar.Text = "Actualizar"
            btnEditar.Text = "Cancelar"
            btnEliminar.Enabled = False
            btnAgregar.Image = My.Resources.aceptar1
            btnEditar.Image = My.Resources.cancelar1
            frmEstado = 2
        End If






    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If frmEstado = 0 Then
            limpiar()
            activar(True)
            txtIdUsuario.Focus()
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