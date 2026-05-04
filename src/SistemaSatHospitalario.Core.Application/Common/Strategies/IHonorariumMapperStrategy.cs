using SistemaSatHospitalario.Core.Domain.Constants;
using System.Linq;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public interface IHonorariumMapperStrategy
    {
        bool CanHandle(string tipoServicio);
        string GetCategory();
    }

    public class RXMapperStrategy : IHonorariumMapperStrategy
    {
        public bool CanHandle(string tipoServicio) => 
            HonorarioConstants.RXPrefixes.Any(p => tipoServicio.ToUpper().Contains(p));
        public string GetCategory() => HonorarioConstants.CategoriaRX;
    }

    public class InformeMapperStrategy : IHonorariumMapperStrategy
    {
        public bool CanHandle(string tipoServicio) => 
            HonorarioConstants.InformePrefixes.Any(p => tipoServicio.ToUpper().Contains(p));
        public string GetCategory() => HonorarioConstants.CategoriaInforme;
    }

    public class CitologiaMapperStrategy : IHonorariumMapperStrategy
    {
        public bool CanHandle(string tipoServicio) => 
            HonorarioConstants.CitologiaPrefixes.Any(p => tipoServicio.ToUpper().Contains(p));
        public string GetCategory() => HonorarioConstants.CategoriaCitologia;
    }

    public class BiopsiaMapperStrategy : IHonorariumMapperStrategy
    {
        public bool CanHandle(string tipoServicio) => 
            HonorarioConstants.BiopsiaPrefixes.Any(p => tipoServicio.ToUpper().Contains(p));
        public string GetCategory() => HonorarioConstants.CategoriaBiopsia;
    }

    public class ConsultaMapperStrategy : IHonorariumMapperStrategy
    {
        public bool CanHandle(string tipoServicio) => 
            HonorarioConstants.ConsultaPrefixes.Any(p => tipoServicio.ToUpper().Contains(p));
        public string GetCategory() => HonorarioConstants.CategoriaConsulta;
    }
}
