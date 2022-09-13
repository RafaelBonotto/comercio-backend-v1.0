﻿using Comum.Dominio.Entidades;
using Dapper;
using Dapper.Contrib.Extensions;
using Usuario.Infra.Conexao.Interfaces;
using Usuario.Infra.Querys;
using Usuario.Infra.Repositorios.Interfaces;

namespace Usuario.Infra.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly IUsuarioConexao _connection;

        public UsuarioRepositorio(IUsuarioConexao connection)
        {
            _connection = connection;
        }

        public async Task<int> InserirUsuario(Comum.Dominio.Entidades.Usuario usuario)
        {
            using var connection = await _connection.GetConnectionAsync();
            // verifica se existe usuario cadastrado com mesmo email
            // criar metodo que adiociona permissao e chamar aqui passando permissao usuario
            var idUsuario = await connection.InsertAsync<Comum.Dominio.Entidades.Usuario>(usuario);
            return idUsuario;
        }

        public async Task<Comum.Dominio.Entidades.Usuario> ObterUsuarioPorEmail(string email)
        {
            using var connection = await _connection.GetConnectionAsync();
            var usuario = await connection.QueryAsync<Comum.Dominio.Entidades.Usuario>(
                    sql: UsuarioQuerys.SELECT_USUARIO_POR_EMAIL,
                    param: new { email });

            if (usuario.Any())
                return usuario.First();

            return null;
        }

        public async Task<List<Permissao>> ObterPermissao(int usuarioId)
        {
            List<Permissao> ret = new();
            using var connection = await _connection.GetConnectionAsync();

            var permissaoId = await connection.QueryAsync<int>(
                    sql: UsuarioQuerys.SELECT_PERMISSAO_ID,
                    param: new { usuario_id = usuarioId });

            if (permissaoId.Any())
                foreach (var id in permissaoId)
                    ret.Add(await connection.GetAsync<Permissao>(id));

            return ret;
        }
    }
}