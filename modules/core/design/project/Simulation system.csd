<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Interface">
      <Name>IDataSet</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IComponentBuilder</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>Controls the construction and storage of components. Does not handle component interactions etc.</Text>
    </Entity>
    <Entity type="Interface">
      <Name>IComponentDatabase</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>Stores component information, not the actual components themselves</Text>
    </Entity>
    <Entity type="Comment">
      <Text>By storing all information in a database we should be able to easily add new simulation extensions that just deal with components.
How about other extensions?</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Build the simulation indirectly out of sets of extensions. The simulation does not exist directly</Text>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Comment" first="2" second="1" />
    <Relation type="Comment" first="4" second="3" />
  </Relations>
  <Positions>
    <Shape>
      <Location left="89" top="254" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="578" top="5" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="818" top="5" />
      <Size width="203" height="65" />
    </Shape>
    <Shape>
      <Location left="578" top="89" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="818" top="89" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="5" top="5" />
      <Size width="329" height="115" />
    </Shape>
    <Shape>
      <Location left="5" top="126" />
      <Size width="329" height="55" />
    </Shape>
    <Connection>
      <StartNode isHorizontal="True" location="25" />
      <EndNode isHorizontal="True" location="25" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="33" />
      <EndNode isHorizontal="True" location="33" />
    </Connection>
  </Positions>
</ClassProject>