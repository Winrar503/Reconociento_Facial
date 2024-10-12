﻿using Face.EntidadesDeNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.AccesoADatos
{
    public class HorariosDAL
    {
        public static async Task<int> CrearAsync(Horarios pHorario)
        {
            try
            {
                using (var bdContext = new BDContexto())
                {
                    bdContext.Add(pHorario);
                    return await bdContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            { 
                throw new Exception("Error al crear el Horario", ex); 
            }
        }
        public static async Task<int> ModificarAsync(Horarios pHorario)
        {
            int result = 0;
            using (var bdContexto = new BDContexto())
            {
                var horarios = await bdContexto.Horarios.FirstOrDefaultAsync(h => h.Id == pHorario.Id);
                if (horarios != null) return 0;

                horarios.HoraEntrada = pHorario.HoraEntrada;
                horarios.HoraSalida = pHorario.HoraSalida;

                result = await bdContexto.SaveChangesAsync();

            }
            return result;
        }
        public static async Task<int> EliminarAsync(Horarios pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                var horarios = await bdContexto.Horarios.FirstOrDefaultAsync(h => h.Id == pHorario.Id);
                if (horarios == null) return 0;

                bdContexto.Horarios.Remove(horarios);
                return await bdContexto.SaveChangesAsync();
            }
        }

        public static async Task<Horarios> ObtenerPorIdAsync(Horarios pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Horarios.FirstOrDefaultAsync(h => h.Id == pHorario.Id);
            }
        }

        public static async Task<List<Horarios>> ObtenerTodosAsync()
        {
            using (var bdContexto = new BDContexto())
            {
                return await bdContexto.Horarios.ToListAsync();
            }
        }

        internal static IQueryable<Horarios> QuerySelect(IQueryable<Horarios> pQuery, Horarios pHorario)
        {
            if (pHorario.Id > 0)
                pQuery = pQuery.Where(s => s.Id == pHorario.Id);


            //if (!string.IsNullOrWhiteSpace(pHorario.HoraEntrada))
            //    pQuery = pQuery.Where(s => s.Tipo.Contains(pAsistencias.Tipo));


            pQuery = pQuery.OrderBy(s => s.Id);


            if (pHorario.Top_Aux > 0)
                pQuery = pQuery.Take(pHorario.Top_Aux).AsQueryable();

            return pQuery;
        }

        public static async Task<List<Horarios>> BuscarAsync(Horarios pHorario)
        {
            using (var bdContexto = new BDContexto())
            {
                var select = bdContexto.Horarios.AsQueryable();
                select = QuerySelect(select, pHorario);
                return await select.ToListAsync();
            }
        }
    }
}