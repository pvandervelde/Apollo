<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxml="urn:schemas-microsoft-com:xslt">
    <xsl:output method="html"
                indent="no"/>

    <xsl:template match="/">

        <xsl:variable name="cov0style"
                      select="'background:#ff0000;text-align:right;font-weight:bold;'"/>
        <xsl:variable name="cov20style"
                      select="'background:#ff6600;text-align:right;font-weight:bold;'"/>
        <xsl:variable name="cov40style"
                      select="'background:#ffcc00;text-align:right;font-weight:bold;'"/>
        <xsl:variable name="cov60style"
                      select="'background:#cc9933;text-align:right;font-weight:bold;'"/>
        <xsl:variable name="cov80style"
                      select="'background:#6699ff;text-align:right;font-weight:bold;'"/>
        <xsl:variable name="cov100style"
                      select="'background:#00cc00;text-align:right;font-weight:bold;'"/>

        <table width="98%"
               cellspacing="0"
               cellpadding="2"
               border="0"
               class="section-table">
            <tr class="sectionheader">
                <td colspan="2">Coverage by assembly</td>
            </tr>

            <xsl:variable name="unique-asms"
                          select="//PartCoverReport/type[not(@asmref=following::type/@asmref)]"/>
            <xsl:for-each select="$unique-asms">
                <xsl:variable name="current-asm"
                              select="./@asmref"/>
                <tr>
                    <xsl:if test="position() mod 2 = 0">
                        <xsl:attribute name="class">section-oddrow</xsl:attribute>
                    </xsl:if>
                    <xsl:element name="td">
                        <xsl:value-of select="$current-asm"/>
                    </xsl:element>

                    <xsl:variable name="codeSize"
                                  select="sum(//PartCoverReport/type[@asmref=$current-asm]/method/code/pt/@len)+0"/>
                    <xsl:variable name="coveredCodeSize"
                                  select="sum(//PartCoverReport/type[@asmref=$current-asm]/method/code/pt[@visit>0]/@len)+0"/>

                    <xsl:element name="td">
                        <xsl:if test="$codeSize=0">
                            <xsl:attribute name="style">
                                <xsl:value-of select="$cov0style"/>
                            </xsl:attribute>
                            0%
                        </xsl:if>

                        <xsl:if test="$codeSize &gt; 0">
                            <xsl:variable name="coverage"
                                          select="ceiling(100 * $coveredCodeSize div $codeSize)"/>
                            <xsl:if test="$coverage = 0">
                                <xsl:attribute name="style">
                                    <xsl:value-of select="$cov0style"/>
                                </xsl:attribute>
                            </xsl:if>
                            <xsl:if test="$coverage &gt; 0 and $coverage &lt; 20">
                                <xsl:attribute name="style">
                                    <xsl:value-of select="$cov20style"/>
                                </xsl:attribute>
                            </xsl:if>
                            <xsl:if test="$coverage &gt;= 20 and $coverage &lt; 40">
                                <xsl:attribute name="style">
                                    <xsl:value-of select="$cov40style"/>
                                </xsl:attribute>
                            </xsl:if>
                            <xsl:if test="$coverage &gt;= 40 and $coverage &lt; 60">
                                <xsl:attribute name="style">
                                    <xsl:value-of select="$cov60style"/>
                                </xsl:attribute>
                            </xsl:if>
                            <xsl:if test="$coverage &gt;= 60 and $coverage &lt; 80">
                                <xsl:attribute name="style">
                                    <xsl:value-of select="$cov80style"/>
                                </xsl:attribute>
                            </xsl:if>
                            <xsl:if test="$coverage &gt;= 80">
                                <xsl:attribute name="style">
                                    <xsl:value-of select="$cov100style"/>
                                </xsl:attribute>
                            </xsl:if>
                            <xsl:value-of select="$coverage"/>%
                        </xsl:if>

                    </xsl:element>
                </tr>
            </xsl:for-each>
        </table>

    </xsl:template>

</xsl:stylesheet>
