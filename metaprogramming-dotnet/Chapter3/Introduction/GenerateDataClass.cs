public class DynamicCar
{
  public System.String Make { get; set; }
  public System.String Model { get; set; }
  public System.Int32 Year { get; set; }
  public System.Int32 MPG { get; private set; }
  public DynamicCar(System.String Make, System.String Model, System.Int32 Year, System.Int32 MPG)
  {
    this.Make = Make;
    this.Model = Model;
    this.Year = Year;
    this.MPG = MPG;
  }
}
