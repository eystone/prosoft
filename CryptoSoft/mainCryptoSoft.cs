using CryptoSoft;
class mainCyptoSoft { 
    public static void Main(string[] args)
    {
        string[] fichier = { "test.json", "crpter.txt" };
        Crpytage  cpytage = new Crpytage();
        int result = cpytage.test(fichier);
        
    }
}