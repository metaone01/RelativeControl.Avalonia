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
        Assert.Equal(["12px","24px36px48px"], result);
    }
    [Fact]
    public void Test_Split_Multiple_Splitters() {
        string text = "12px 24px,36px|48px;64px";
        char[] splitters = [' ',',','|',';'];
        string[] result = Splitters.Split(text, splitters);
        Assert.Equal(["12px","24px","36px","48px","64px"], result);
    }
    [Fact]
    public void Test_Split_Multiple_Splitters_In_A_Row() {
        string text = "12px ,24px|;36px48px";
        char[] splitters = [' ',',','|',';'];
        string[] result = Splitters.Split(text, splitters);
        Assert.Equal(["12px","24px","36px48px"], result);
    }
    [Fact]
    public void Test_Split_Begin_End_With_Splitter() {
        string text = ", ,12px24px , 36px48px, ,";
        char[] splitters = [',',' '];
        string[] result = Splitters.Split(text, splitters);
        Assert.Equal(["12px24px","36px48px"], result);
    }
}