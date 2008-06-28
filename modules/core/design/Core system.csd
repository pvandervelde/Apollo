<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Interface">
      <Name>IService</Name>
      <Access>Public</Access>
      <Location left="887" top="181" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Class">
      <Name>BootStrapper</Name>
      <Access>Public</Access>
      <Location left="620" top="10" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Interface">
      <Name>IAppDomainBuilder</Name>
      <Access>Public</Access>
      <Location left="407" top="10" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IUserInterface</Name>
      <Access>Public</Access>
      <Location left="99" top="136" />
      <Size width="162" height="96" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IProject</Name>
      <Access>Public</Access>
      <Location left="275" top="403" />
      <Size width="162" height="86" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IVisualizer</Name>
      <Access>Public</Access>
      <Location left="117" top="650" />
      <Size width="162" height="108" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IData</Name>
      <Access>Public</Access>
      <Location left="297" top="650" />
      <Size width="162" height="109" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IGenerator</Name>
      <Access>Public</Access>
      <Location left="487" top="650" />
      <Size width="162" height="108" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICoreSystem</Name>
      <Access>Public</Access>
      <Location left="564" top="181" />
      <Size width="162" height="90" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IProjectPlugin</Name>
      <Access>Public</Access>
      <Location left="597" top="403" />
      <Size width="162" height="90" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IPluginRepository</Name>
      <Access>Public</Access>
      <Location left="947" top="432" />
      <Size width="162" height="126" />
      <Collapsed>False</Collapsed>
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Association" first="4" second="5">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>302</X>
        <Y>514</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="4" second="6">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="4" second="7">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>403</X>
        <Y>515</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="9" second="4">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="7" second="9">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>678</X>
        <Y>712</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="3" second="4">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>300</X>
        <Y>378</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="8" second="3">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>501</X>
        <Y>220</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>293</X>
        <Y>173</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="1" second="8">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="8" second="2">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>599</X>
        <Y>156</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="8" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>681</X>
        <Y>305</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Generalization" first="10" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1028</X>
        <Y>271</Y>
      </BendPoint>
    </Relation>
  </Relations>
</ClassProject>