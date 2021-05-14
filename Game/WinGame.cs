using Godot;

public partial class WinGame : Node {
  private TextureButton replayBtn;
  private TextureButton exitBtn;
  private Game game;

  public override void _Ready() {
    // GetNode("Menu/CenterRow/Buttons").Connect("pressed", this, nameof(OnButtonPressed), new Godot.Collections.Array() { button.scene_to_load });
    this.game = (Game)GetNode("/root/Game");
    this.replayBtn = (TextureButton)GetNode("UI/ReplayBtn");
    this.exitBtn = (TextureButton)GetNode("UI/ExitBtn");

    replayBtn.Connect("pressed", this, nameof(replayButtonPressed));
    exitBtn.Connect("pressed", this, nameof(exitButtonPressed));
  }

  private void replayButtonPressed() {
    game.startGame();
  }

  private void exitButtonPressed() {
    game.exitGame();
  }
}