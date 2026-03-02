using System;

public abstract class Animal
{
    public abstract string Speak();
}

public sealed class Cat : Animal
{
    public override string Speak() => "Meow";
}

public sealed class Dog : Animal
{
    public override string Speak() => "Woof";
}

public static class Program
{
    public static void Main()
    {
        Animal a = new Cat();
        Console.WriteLine(a.Speak());
        a = new Dog();
        Console.WriteLine(a.Speak());
    }
}
