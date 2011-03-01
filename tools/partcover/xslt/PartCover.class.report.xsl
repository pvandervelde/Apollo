<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxml="urn:schemas-microsoft-com:xslt">
<xsl:output method="html" indent="no"/>
<xsl:param name="applicationPath"/>

<xsl:template match="/">

	<xsl:variable name="cov0style" select="'background:#ff0000;text-align:right;font-weight:bold;'"/>
	<xsl:variable name="cov20style" select="'background:#ff6600;text-align:right;font-weight:bold;'"/>
	<xsl:variable name="cov40style" select="'background:#ffcc00;text-align:right;font-weight:bold;'"/>
	<xsl:variable name="cov60style" select="'background:#cc9933;text-align:right;font-weight:bold;'"/>
	<xsl:variable name="cov80style" select="'background:#6699ff;text-align:right;font-weight:bold;'"/>
	<xsl:variable name="cov100style" select="'background:#00cc00;text-align:right;font-weight:bold;'"/>
			<script>
				function toggleDiv( imgId, divId )
				{
					eDiv = document.getElementById( divId );
					eImg = document.getElementById( imgId );
					
					if ( eDiv.style.display == "none" )
					{
						eDiv.style.display = "block";
						eImg.src = "<xsl:value-of select="$applicationPath"/>/images/arrow_minus_small.gif";
					}
					else
					{
						eDiv.style.display = "none";
						eImg.src = "<xsl:value-of select="$applicationPath"/>/images/arrow_plus_small.gif";
					}
				}
			</script>
	<table width="98%" cellspacing="0" cellpadding="2" border="0" class="section-table">
	
		<tr class="sectionheader"><td colspan="2">Part Cover - Coverage by class</td></tr>
		
		<xsl:for-each select="//PartCoverReport/type">
			<tr>
				<xsl:variable name="className" select="@name"/>
				<td style="width: 15%; vertical-align: top;">
				  <xsl:attribute name="class">section-data</xsl:attribute>
				  <xsl:element name="input">
				    <xsl:attribute name="type">image</xsl:attribute>
						<xsl:attribute name="onclick">javascript:toggleDiv('img<xsl:value-of select="$className"/>', 'divDetails<xsl:value-of select="$className"/>');</xsl:attribute>
				    <xsl:attribute name="id">img<xsl:value-of select="$className"/></xsl:attribute>
            <xsl:attribute name="src">http://development.sureflix.com/ccnet/images/arrow_plus_small.gif</xsl:attribute>
				  </xsl:element>
					<xsl:value-of select="$className"/>
				</td>
				
				<xsl:variable name="codeSize" select="sum(./method/code/pt/@len)+0"/>
				<xsl:variable name="coveredCodeSize" select="sum(./method/code/pt[@visit>0]/@len)+0"/>
				
				<td style="width: 85%">
				  <div>
					<xsl:if test="$codeSize=0">
						<xsl:attribute name="style"><xsl:value-of select="$cov0style"/></xsl:attribute>
						0%
					</xsl:if>

					<xsl:if test="$codeSize &gt; 0">
						<xsl:variable name="coverage" select="ceiling(100 * $coveredCodeSize div $codeSize)"/>
		    		<xsl:if test="$coverage = 0"><xsl:attribute name="style"><xsl:value-of select="$cov0style"/></xsl:attribute></xsl:if>        				
						<xsl:if test="$coverage &gt; 0 and $coverage &lt; 20"><xsl:attribute name="style"><xsl:value-of select="$cov20style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 20 and $coverage &lt; 40"><xsl:attribute name="style"><xsl:value-of select="$cov40style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 40 and $coverage &lt; 60"><xsl:attribute name="style"><xsl:value-of select="$cov60style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 60 and $coverage &lt; 80"><xsl:attribute name="style"><xsl:value-of select="$cov80style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 80"><xsl:attribute name="style"><xsl:value-of select="$cov100style"/></xsl:attribute></xsl:if>
						<xsl:value-of select="$coverage"/>%
					</xsl:if>
					</div>
          <div style="display:none">
  					<xsl:attribute name="id">divDetails<xsl:value-of select="$className"/></xsl:attribute>
  					<table border="0" cell-padding="6" cell-spacing="0" width="100%">
						  <tr class="sectionheader">
                <td style="width:80%;">Method name</td>
                <td style="width:10%;">Code size</td>
                <td style="width:10%;">Covered</td>
              </tr>
  						<xsl:for-each  select="./method">					
  						  <xsl:variable name="methodCodeSize" select="sum(./code/pt/@len)+0"/>
				        <xsl:variable name="methodCoveredCodeSize" select="sum(./code/pt[@visit>0]/@len)+0"/>
  						  <tr>
  						  	<xsl:if test="position() mod 2 = 0">
            				<xsl:attribute name="class">section-oddrow</xsl:attribute>	
            			</xsl:if>
                  <td><xsl:value-of select="@name"/></td>
                  <td style="text-align: center;"><xsl:value-of select="$methodCodeSize"/></td>
                  <td>
                  	<xsl:if test="$methodCodeSize &gt; $methodCoveredCodeSize"><xsl:attribute name="style"><xsl:value-of select="$cov0style"/>text-align: center;</xsl:attribute></xsl:if>
                  	<xsl:if test="$methodCodeSize = $methodCoveredCodeSize"><xsl:attribute name="style"><xsl:value-of select="$cov100style"/>text-align: center;</xsl:attribute></xsl:if>
                    <xsl:value-of select="$methodCoveredCodeSize"/>
                  </td>
                </tr>
  						</xsl:for-each>
  						
  					</table>
  				</div>	
				</td>
			</tr>
		</xsl:for-each>
	</table>
	
</xsl:template>

</xsl:stylesheet>
