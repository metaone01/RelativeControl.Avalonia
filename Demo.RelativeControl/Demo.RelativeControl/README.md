This is a instruction of relative features used in this demo.

[MainWindow.axaml](./MainWindow.axaml)
```xaml
<Grid RowDefinitions="Auto,4,*" ColumnDefinitions="Auto,4,*">
        <ListBox Grid.Row="0" Grid.Column="0"
                 r:Relative.Width="50pw"  // This sets its width to 50% parent width
                 r:Relative.Height="50ph"> // This sets its height to 50% parent height
            <ListBoxItem>
                <TextBlock Name="Block"
                           Text="{Binding #Right.Bounds.Width}"
                           r:Relative.Width="70pw" /> // This sets its width to 70% parent width
            </ListBoxItem>
            <ListBoxItem>
                <TextBlock Text="{r:RelativeBinding {Binding #Block.Text},50%}" // This sets its content to 50% of #Block.Text(means the Text property's value of TextBlock named Block above)
                           r:Relative.Width="30vw" // This sets its width to 30% window width
                           Height="{r:RelativeBinding {Binding #Bottom.Bounds.Height},50%}" /> // This binds its height to 50% of #Bottom.Bounds.Height
            </ListBoxItem>
        </ListBox>
```