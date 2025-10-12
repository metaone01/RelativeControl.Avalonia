using RelativeControl.Avalonia;

namespace Test.RelativeControl;


public class SplittersTest {
    [Fact]
    public void Test_Split_NoSplit() {
        string text = "12px24px36px48px";
        char[] splitters = [' '];
        string[] result = Splitters.Split(text, splitters);
        Assert.Equal(["12px24px36px48px"], result);
    }
    [Fact]
    public void Test_Split_SplitSingle() {
        string text = "12px 24px36px48px";
        char[] splitters = [' '];
        string[] result = Splitters.Split(text, splitters);
        Assert.Equal(["12px", "24px36px48px"], result);
    }

    [Fact]
    public void Test_Split_Multiple_Splitters() {
        string text = "12px 24px,36px|48px;64px";
        char[] splitters = [' ', ',', '|', ';'];
        string[] result = Splitters.Split(text, splitters);
        Assert.Equal(["12px", "24px", "36px", "48px", "64px"], result);
    }

    [Fact]
    public void Test_Split_Begins_Ends_With_Splitter() {
        string text = ",12px24px , 36px48px,";
        char[] splitters = [',', ' '];
        Assert.Throws<FormatException>(() => Splitters.Split(text, splitters));
    }

    [Fact]
    public void Test_Split_Empty_Value_Should_Throw_FormatException() {
        string text = ", ,12px24px , 36px48px, ,";
        char[] splitters = [',', ' '];
        Assert.Throws<FormatException>(() => Splitters.Split(text, splitters));
    }

    [Fact]
    public void Test_Split_Empty() {
        string text = "   ";
        char[] splitters = [',', ' '];
        Assert.Throws<FormatException>(() => Splitters.Split(text, splitters));
    }

    [Fact]
    public void Test_Split_Only_Splitter() {
        string text = " ,  ";
        char[] splitters = [',', ' '];
        Assert.Throws<FormatException>(() => Splitters.Split(text, splitters));
    }

    [Fact]
    public void Test_Split_Ends_With_Empty() {
        string text = " 1 ,  ";
        char[] splitters = [',', ' '];
        Assert.Throws<FormatException>(() => Splitters.Split(text, splitters));
    }

    [Fact]
    public void Test_Split_Begins_With_Empty() {
        string text = " ,  1";
        char[] splitters = [',', ' '];
        Assert.Throws<FormatException>(() => Splitters.Split(text, splitters));
    }

    [Fact]
    public void Test_Split_With_Empty_Value() {
        string text = " 1,,  1";
        char[] splitters = [',', ' '];
        Assert.Throws<FormatException>(() => Splitters.Split(text, splitters));
    }

    [Fact]
    public void Test_Split_Space_Split() {
        string text = "  1 23 4 5 6  ";
        char[] splitters = [',', ' '];
        string[] result = Splitters.Split(text, splitters);
        Assert.Equal(["1", "23", "4", "5", "6"], result);
    }
}