<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Interface">
      <Name>ITag</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IGoal</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IConstraint</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IRequirement</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Interface">
      <Name>IDeliverable</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>Do we need a language for tags so that we can combine them? Is this specific for each tag type?</Text>
    </Entity>
    <Entity type="Interface">
      <Name>ICondition</Name>
      <Access>Public</Access>
    </Entity>
    <Entity type="Comment">
      <Text>Have meta data proxies that allow storing tags as meta data?</Text>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Generalization" first="2" second="0" />
    <Relation type="Generalization" first="1" second="0" />
    <Relation type="Generalization" first="3" second="0" />
    <Relation type="Generalization" first="4" second="0" />
    <Relation type="Comment" first="5" second="0" />
    <Relation type="Generalization" first="6" second="0" />
    <Relation type="Comment" first="7" second="0" />
  </Relations>
  <Positions>
    <Shape>
      <Location left="480" top="145" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="869" top="341" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="271" top="341" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="447" top="341" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="689" top="341" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="759" top="2" />
      <Size width="162" height="89" />
    </Shape>
    <Shape>
      <Location left="94" top="341" />
      <Size width="162" height="65" />
    </Shape>
    <Shape>
      <Location left="240" top="2" />
      <Size width="162" height="72" />
    </Shape>
    <Connection>
      <StartNode isHorizontal="False" location="120" />
      <EndNode isHorizontal="True" location="41" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="77" />
      <EndNode isHorizontal="True" location="20" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="92" />
      <EndNode isHorizontal="False" location="59" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="70" />
      <EndNode isHorizontal="True" location="45" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="44" />
      <EndNode isHorizontal="False" location="113" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="False" location="77" />
      <EndNode isHorizontal="True" location="20" />
    </Connection>
    <Connection>
      <StartNode isHorizontal="True" location="45" />
      <EndNode isHorizontal="False" location="58" />
    </Connection>
  </Positions>
</ClassProject>