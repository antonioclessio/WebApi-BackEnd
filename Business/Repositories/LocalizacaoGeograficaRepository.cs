using GrupoOrto.ERP.Entities.Entity.LocalizacaoGeografica;
using GrupoOrto.ERP.Entities.Entity.Table;
using GrupoOrto.ERP.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GrupoOrto.ERP.Business.Repositories
{
    public class LocalizacaoGeograficaRepository : BaseRepository
    {
        protected override int IdAplicacao => 0;

        /// <summary>
        /// Busca a localização geográfica a partir do CEP.
        /// </summary>
        /// <param name="cep"></param>
        /// <returns></returns>
        public LocalizacaoGeograficaResult GetByCEP(string cep)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.LocalizacaoGeografica.FirstOrDefault(a => a.CEP == cep);
                if (entity == null) return null;
                return GetByKey(entity.IdLocalizacaoGeografica);
            }
        }

        /// <summary>
        /// Retorna a localização geográfica com base na key
        /// </summary>
        /// <param name="key">Chave para retornar a localização geográfica</param>
        /// <returns></returns>
        public LocalizacaoGeograficaResult GetByKey(int key)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.LocalizacaoGeografica.First(a => a.IdLocalizacaoGeografica == key);
                var bairroEntity = context.Bairro.First(a => a.IdBairro == entity.IdBairro);
                var cidadeEntity = context.Cidade.First(a => a.IdCidade == entity.IdCidade);
                var estadoEntity = context.Estado.First(a => a.IdEstado == cidadeEntity.IdEstado);

                var localizacaoGeografica = new LocalizacaoGeograficaResult
                {
                    IdLocalizacaoGeografica = entity.IdLocalizacaoGeografica,
                    Bairro = bairroEntity.Nome,
                    Cidade = cidadeEntity.Nome,
                    Estado = estadoEntity.Sigla,
                    Logradouro = entity.Logradouro,
                    CEP = entity.CEP
                };

                return localizacaoGeografica;
            }
        }

        /// <summary>
        /// Retorna uma cidade a partir do nome e do estado.
        /// </summary>
        /// <param name="uf"></param>
        /// <param name="cidade"></param>
        /// <returns></returns>
        public Cidade GetCidade(string uf, string cidade)
        {
            using (var context = new DatabaseContext())
            {
                var entityEstado = context.Estado.FirstOrDefault(a => a.Sigla == uf);
                var entityCidade = context.Cidade.FirstOrDefault(a => a.IdEstado == entityEstado.IdEstado && a.Nome == cidade);

                return entityCidade;
            }
        }

        /// <summary>
        /// Grava a localização geográfica retornada pela consulta no Cep Aberto
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public LocalizacaoGeograficaResult SaveFromCEPAberto(CEPAbertoResult view, bool fromJob = false)
        {
            using (var context = new DatabaseContext())
            {
                if (view.estado == null) return null;

                var entityEstado = context.Estado.First(a => a.Sigla.ToUpper().Equals(view.estado));
                var entityCidade = context.Cidade.FirstOrDefault(a => a.Nome.ToLower().Trim() == view.cidade.ToLower().Trim());
                if (entityCidade == null)
                {
                    entityCidade = new Cidade();
                    entityCidade.IdEstado = entityEstado.IdEstado;
                    entityCidade.Nome = view.cidade;
                    entityCidade.CodigoIBGE = view.ibge;
                    entityCidade.DataHoraCadastro = DateTime.Now;
                    entityCidade.Status = (int)DefaultStatusEnum.Ativo;
                    entityCidade.Revisao = false;

                    context.Set<Cidade>().Add(entityCidade);
                }
                else
                {
                    entityCidade.CodigoIBGE = view.ibge;
                    entityCidade.Nome = view.cidade;
                    entityCidade.Revisao = false;

                    context.Entry(entityCidade).State = EntityState.Modified;
                }

                var entityBairro = context.Bairro.FirstOrDefault(a => a.IdCidade == entityCidade.IdCidade && a.Nome == view.bairro);
                if (entityBairro == null && view.bairro != null)
                {
                    entityBairro = new Bairro();
                    entityBairro.IdCidade = entityCidade.IdCidade;
                    entityBairro.Nome = view.bairro;
                    entityBairro.DataHoraCadastro = DateTime.Now;
                    entityBairro.Status = (int)DefaultStatusEnum.Ativo;

                    context.Set<Bairro>().Add(entityBairro);
                }

                int idUsuario = fromJob ? 1 : GetLoggedUser().IdUsuario;

                var entity = new LocalizacaoGeografica();
                entity.IdUsuarioCadastro = idUsuario;
                entity.IdUsuarioAlteracao = idUsuario;
                entity.DataHoraAlteracao = DateTime.Now;
                entity.DataHoraCadastro = DateTime.Now;
                entity.Status = (int)DefaultStatusEnum.Ativo;
                entity.Altitude = view.altitude;
                entity.Latitude = view.latitude;
                entity.Longitude = view.longitude;
                entity.Logradouro = view.logradouro;

                if (view.ddd != null)
                    entity.DDD = int.Parse(view.ddd);

                entity.IdCidade = entityCidade.IdCidade;
                entity.IdEstado = entityCidade.IdEstado;
                entity.IdRegiao = entityEstado.IdRegiao;

                if (entityBairro != null)
                    entity.IdBairro = entityBairro.IdBairro;

                entity.CEP = view.cep;
                entity.IdPais = 1;

                context.Set<LocalizacaoGeografica>().Add(entity);

                context.SaveChanges();

                return GetByKey(entity.IdLocalizacaoGeografica);
            }
        }

        /// <summary>
        /// Retorna todos os estados cadastrados.
        /// </summary>
        /// <returns></returns>
        public List<Estado> GetEstados()
        {
            using (var context = new DatabaseContext())
            {
                var listEntity = context.Estado.OrderBy(a => a.Nome).ToList();
                return listEntity;
            }
        }

        /// <summary>
        /// Retorna as cidades de um determinado estado, e também permite filtro entre as cidades.
        /// </summary>
        /// <param name="idEstado">Estado que deverá retornar as cidades.</param>
        /// <param name="text">Auto complete entre o nome das cidadesd</param>
        /// <returns></returns>
        public List<Cidade> GetCidades(int idEstado, string text = null)
        {
            using (var context = new DatabaseContext())
            {
                var listCidades = context.Cidade.Where(a => a.IdEstado == idEstado && (text == null || a.Nome.Contains(text))).OrderBy(a => a.Nome).ToList();
                return listCidades;
            }
        }
    }
}
