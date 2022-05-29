using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SubstanceClassifier
{
    /// <summary>
    /// Medžiaga
    /// </summary>
    public class Substance
    {
        /// <summary>
        /// Medžiagos pavadinimas
        /// </summary>
        public string SubstanceName { get; set; }

        /// <summary>
        /// Medžiagos apibūdinimas
        /// </summary>
        public string SubstanceDescription { get; set; }

        /// <summary>
        /// Informacijos apie medžiagą URL.
        /// </summary>
        public string SubstanceInfoUrl { get; set; }

        /// <summary>
        /// Medžiagos EC Numeris
        /// </summary>
        public string ECNumber { get; set; }

        /// <summary>
        /// Medžiagos CAS Numeris (duomenų bazės raktas)
        /// </summary>
        [Key]
        public string CASNumber { get; set; }

        /// <summary>
        /// Medžiagos klasifikacija (atskirta kableliais)
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// Medžiagos šaltinis
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Papildomos informacijos apie medžiagą URL.
        /// </summary>
        public string DetailsUrl { get; set; }

        /// <summary>
        /// Patikrina, ar yra visa reikalinga informacija apie medžiagą.
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return !string.IsNullOrEmpty(CASNumber) && !string.IsNullOrEmpty(Classification) && !string.IsNullOrEmpty(DetailsUrl);
        }
    }

    /// <summary>
    /// Palygina ar medžiagos yra tapačios.
    /// </summary>
    public class SubstanceComparer : EqualityComparer<Substance>
    {
        public override bool Equals(Substance x, Substance y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.CASNumber == y.CASNumber;
        }

        public override int GetHashCode([DisallowNull] Substance obj)
        {
            return obj.CASNumber.GetHashCode();
        }
    }
}

