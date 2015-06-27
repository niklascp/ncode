using FileHelpers;

namespace nCode.Catalog.Models
{
    /* HEADER 
     * Økonomi- og Indenrigsministeriets Kommunale N(ø)gletal;;
     * ;;
     * ;Kom.nr;2015
     * ;;
     */
    [DelimitedRecord(";"), IgnoreFirst(4)]
    public class MunicipalityPopulation
    {
        public string MunicipalityName { get; set; }

        public string MunicipalityCode { get; set; }
        
        public int Population { get; set; }
    }
}
