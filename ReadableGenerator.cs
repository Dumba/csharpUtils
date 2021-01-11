namespace ConsoleApplication1
{
  public class ReadableGenerator
  {
    public static char[] Consonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z', };
    public static char[] Vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', };

    public Generator()
    {
      _random = new Random();
    }

    private Random _random;

    public string Generate(int length)
    {
      int isSwitched = 0;
      int lastSwitchedOn = -5;
      var result = new char[length];
      
      for (int i = 0; i < length; i++)
      {
        // multiple vowels/consonants in a row
        if (_random.Next(5) == 0 && lastSwitchedOn != i - 1)
        {
          isSwitched = (isSwitched + 1) % 2;
          lastSwitchedOn = i;
        }

        // select vowels || consonants
        var charArray = i % 2 == isSwitched
          ? Consonants
          : Vowels;

        // add char
        result[i] = charArray[_random.Next(charArray.Length)];
      }

      return new string(result);
    }
  }
}