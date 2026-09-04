namespace SecchiServicios.Pdf.Services;

public class NumeroALetras
{
    private static readonly string[] Unidades =
    {
        "CERO",
        "UNO",
        "DOS",
        "TRES",
        "CUATRO",
        "CINCO",
        "SEIS",
        "SIETE",
        "OCHO",
        "NUEVE",
        "DIEZ",
        "ONCE",
        "DOCE",
        "TRECE",
        "CATORCE",
        "QUINCE",
        "DIECISÉIS",
        "DIECISIETE",
        "DIECIOCHO",
        "DIECINUEVE",
        "VEINTE"
    };

    private static readonly string[] Decenas =
    {
        "",
        "",
        "",
        "TREINTA",
        "CUARENTA",
        "CINCUENTA",
        "SESENTA",
        "SETENTA",
        "OCHENTA",
        "NOVENTA"
    };

    private static readonly string[] Centenas =
    {
        "",
        "CIENTO",
        "DOSCIENTOS",
        "TRESCIENTOS",
        "CUATROCIENTOS",
        "QUINIENTOS",
        "SEISCIENTOS",
        "SETECIENTOS",
        "OCHOCIENTOS",
        "NOVECIENTOS"
    };

    public static string Convertir(decimal numero)
    {
        if (numero < 0)
            return "MENOS " + Convertir(-numero);

        numero = Math.Round(numero, 2);

        long parteEntera = (long)Math.Truncate(numero);
        int centavos = (int)Math.Round((numero - parteEntera) * 100);

        string resultado;

        if (parteEntera == 0)
        {
            resultado = "CERO PESOS";
        }
        else
        {
            resultado = ConvertirNumero(parteEntera);

            if (parteEntera == 1)
                resultado += " PESO";
            else
                resultado += " PESOS";
        }

        if (centavos > 0)
        {
            resultado += $" CON {centavos:00} CENTAVOS";
        }

        return resultado;
    }

    private static string ConvertirNumero(long numero)
    {
        if (numero <= 20)
            return Unidades[numero];

        if (numero < 100)
        {
            int decena = (int)(numero / 10);
            int unidad = (int)(numero % 10);

            return unidad == 0
                ? Decenas[decena]
                : $"{Decenas[decena]} Y {Unidades[unidad]}";
        }

        if (numero < 1000)
        {
            int centena = (int)(numero / 100);
            int resto = (int)(numero % 100);

            if (numero == 100)
                return "CIEN";

            return resto == 0
                ? Centenas[centena]
                : $"{Centenas[centena]} {ConvertirNumero(resto)}";
        }

        if (numero < 1_000_000)
        {
            long miles = numero / 1000;
            long resto = numero % 1000;

            string resultado = miles == 1
                ? "MIL"
                : $"{ConvertirNumero(miles)} MIL";

            return resto == 0
                ? resultado
                : $"{resultado} {ConvertirNumero(resto)}";
        }

        if (numero < 1_000_000_000)
        {
            long millones = numero / 1_000_000;
            long resto = numero % 1_000_000;

            string resultado = millones == 1
                ? "UN MILLÓN"
                : $"{ConvertirNumero(millones)} MILLONES";

            return resto == 0
                ? resultado
                : $"{resultado} {ConvertirNumero(resto)}";
        }

        throw new ArgumentOutOfRangeException(
            nameof(numero),
            "El número es demasiado grande.");
    }
}