using GrupoOrto.ERP.Business.Interfaces;
using GrupoOrto.ERP.Entities.Entity.Clinica_Usuario;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System.Collections.Generic;
using System.Linq;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class Clinica_UsuarioRepository : BaseRepository, IBaseRepository
    {
        protected override int IdAplicacao => (int)AplicacaoEnum.Clinica;

        /// <summary>
        /// Retorna a lista de entidades filtrado pelo código do usuário.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<ClinicaUsuarioResult> GetByUsuario(int idUsuario, int? idClinica = null)
        {
            using (var context = new DatabaseContext())
            {
                var result = (from clinicaUsuario in context.Clinica_Usuario
                              join clinica in context.Clinica on clinicaUsuario.IdClinica equals clinica.IdClinica
                              join grupo in context.UsuarioGrupo_Permissao on clinicaUsuario.IdUsuarioGrupo equals grupo.IdUsuarioGrupo
                              where clinicaUsuario.IdUsuario == idUsuario
                                 && (idClinica.HasValue == false || clinicaUsuario.IdClinica == idClinica.Value)
                              select new ClinicaUsuarioResult
                              {
                                  IdClinica = clinicaUsuario.IdClinica,
                                  IdUsuario = clinicaUsuario.IdUsuario,
                                  Sigla = clinica.Sigla,
                                  NomeFantasia = clinica.NomeFantasia,
                                  IdUsuarioGrupo = clinicaUsuario.IdUsuarioGrupo,
                                  IdAplicacao = grupo.IdAplicacao,
                                  Permissoes = grupo.Permissoes
                              })
                              .ToList();

                return result;
            }
        }
    }
}
