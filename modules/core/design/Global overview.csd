<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Class">
      <Name>Project</Name>
      <Access>Public</Access>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Links to a data generation component and a data set component</Text>
    </Entity>
    <Entity type="Comment">
      <Text>Project consist of project meta data and a collection of data sets and associated data generators

Note that there should be boundaries between the different parts to ensure safety and restartability. Global stystem (loaders and services) and the UI should be shielded at all times

All component &amp; object loading happens in the project!. Individual systems do not load components, can only request. That way we can keep visualizers, data and generator separate with single loader.</Text>
    </Entity>
    <Entity type="Interface">
      <Name>IProjectPlugin</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IDataGenerator</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IDataSet</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>GLOBAL SYSTEM</Text>
    </Entity>
    <Entity type="Interface">
      <Name>IVisualizer</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>UI</Text>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Comment" first="1" second="0" />
    <Relation type="Association" first="3" second="0">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="6" second="3" />
    <Relation type="Association" first="0" second="4">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="4" second="5">
      <Direction>SourceDestination</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="0">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="5">
      <Direction>SourceDestination</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="8" second="0" />
  </Relations>
  <Positions>
    <Shape>
      <Location left="535" top="357" />
      <Size width="162" height="85" />
    </Shape>
    <Shape>
      <Location left="535" top="204" />
      <Size width="162" height="72" />
    </Shape>
    <Shape>
      <Location left="4" top="5" />
      <Size width="294" height="225" />
    </Shape>
    <Shape>
      <Location left="833" top="370" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="561" top="630" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="248" top="561" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="884" top="158" />
      <Size width="111" height="50" />
    </Shape>
    <Shape>
      <Location left="72" top="400" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="319" top="158" />
      <Size width="79" height="72" />
    </Shape>
    <Connection>
      <StartNode isHorizontal="False" location="82" />
      <EndNode isHorizontal="False" location="82" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="24" />
      <EndNode isHorizontal="True" location="37" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="97" />
      <EndNode isHorizontal="False" location="45" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="133" />
      <EndNode isHorizontal="False" location="106" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="35" />
      <EndNode isHorizontal="True" location="29" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="31" />
      <EndNode isHorizontal="True" location="42" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="114" />
      <EndNode isHorizontal="True" location="30" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="42" />
      <EndNode isHorizontal="True" location="14" />
    </Connection>
  </Positions>
</ClassProject>