<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Interface">
      <Name>IProject</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IComponentBuilder</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IObjectLoader</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IProjectExtension</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>The project system is responsible for tracking the project tree which holds the connections between the different 'data units'. Creation of sub-unit, sibbling unit etc. always runs through project!

</Text>
    </Entity>
    <Entity type="Interface">
      <Name>IDataUnitGraph</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IGraphNodeCreator</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IDataUnit</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>Note that the 'data-unit' graph only needs to hold the 'description' of each initial data set. The descriptions hold all the variables, geometry and other data which can be changed to obtain another 'data-unit'. 

The generator, visualization and actual data sets can be linked to the 'data-unit'-description but they don't necessarily need to be in the tree.

By separating the actual generated data from the 'description' data we can allow modification (copy/clone/modify/edit etc.) of a 'data-unit' without having to digg through all the generated data. This also forces components / users to indicate what data should be considered in the tree and what data doesn't need to be considered.</Text>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Association" first="1" second="2">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="5">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="6">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="6" second="5">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="5" second="0">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="1">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="3">
      <Direction>None</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
  </Relations>
  <Positions>
    <Shape>
      <Location left="773" top="208" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="773" top="17" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="1036" top="17" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="1060" top="208" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="5" top="5" />
      <Size width="282" height="147" />
    </Shape>
    <Shape>
      <Location left="773" top="376" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="508" top="376" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="642" top="572" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="5" top="156" />
      <Size width="282" height="261" />
    </Shape>
    <Connection>
      <StartNode isHorizontal="True" location="27" />
      <EndNode isHorizontal="True" location="27" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="30" />
      <EndNode isHorizontal="False" location="93" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="30" />
      <EndNode isHorizontal="False" location="78" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="28" />
      <EndNode isHorizontal="True" location="28" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="90" />
      <EndNode isHorizontal="False" location="90" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="89" />
      <EndNode isHorizontal="False" location="89" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="30" />
      <EndNode isHorizontal="True" location="30" />
    </Connection>
  </Positions>
</ClassProject>