
DROP PROCEDURE IF EXISTS ProductLoadAllPaged;
DELIMITER $$
CREATE PROCEDURE `ProductLoadAllPaged`(
		`CategoryIds`										text,				#a list of category IDs (comma-separated list). e.g. 1,2,3
		`ManufacturerId`									int,
		`StoreId`											int,
		`VendorId`											int,
		`WarehouseId`										int,
		`ProductTypeId`										int, 				#product type identifier, null - load all products
		`ProductApprovalStatusId`										int, 				
		`VisibleIndividuallyOnly` 							bool, 				#0 - load all products , 1 - "visible indivially" only
		`MarkedAsNewOnly`									bool, 				#0 - load all products , 1 - "marked as new" only
		`ProductTagId`										int,
		`FeaturedProducts`									bool,				#0 featured only , 1 not featured only, null - load all products
		`VendorOnline`										bool,				#vendor online filter
		`PriceMin`											decimal(18, 4),
		`PriceMax`											decimal(18, 4),
		`Keywords`											text,
		`SearchDescriptions` 								bool, 				#a value indicating whether to search by a specified "keyword" in product descriptions
		`SearchManufacturerPartNumber` 						bool, 				# a value indicating whether to search by a specified "keyword" in manufacturer part number
		`SearchSku`											bool, 				#a value indicating whether to search by a specified "keyword" in product SKU
		`SearchProductTags`  								bool, 				#a value indicating whether to search by a specified "keyword" in product tags
		`UseFullTextSearch`  								bool,
		`FullTextMode`										int, 				#0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
		`FilteredSpecs`										text,				#filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
		`LanguageId`										int,
		`OrderBy`											int, 				#0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
		`AllowedCustomerRoleIds`							text,				#a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
		`PageIndex`											int, 
		`PageSize`											int,
		`ShowHidden`										bool,
		`OverridePublished`									bool, 				#null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" products
		`LoadFilterableSpecificationAttributeOptionIds` 	bool, 				#a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
		`RatingMin`											decimal(18, 4),
		`RatingMax`											decimal(18, 4),
		`Latitude`											decimal(18, 7),
		`Longitude`											decimal(18, 7),
		`CoverageDistance`									decimal(18, 7),
	out	`FilterableSpecificationAttributeOptionIds` 		text, 				#the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	out	`TotalRecords`										int
)
    READS SQL DATA 
    SQL SECURITY INVOKER
BEGIN
	DECLARE `SearchKeywords` bit default false;
	DECLARE `sql_orderby` text;
	
    Set @sql_command = '';
    
    drop temporary TABLE if exists `KeywordProducts`;
    drop temporary TABLE if exists `DisplayOrderTmp`;
    
    /* Products that filtered by keywords */
	CREATE temporary TABLE `KeywordProducts`
	(
		`ProductId` int NOT NULL
	);
    
	Set @Keywords = trim(COALESCE(`Keywords`, ''));
    SET @OriginalKeywords = @Keywords;
    
    IF @Keywords != '' then
		SET `SearchKeywords` = true;
        IF `UseFullTextSearch` then
			#remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '');
			SET @Keywords = REPLACE(@Keywords, '"', '');
            set @Keywords = concat(' ', @Keywords);
            
            IF `FullTextMode` = 0 then
				SET @Keywords = concat(' "', @Keywords, '*" ');
			else
				#5 - using CONTAINS and OR with <prefix_term>
				#10 - using CONTAINS and AND with <prefix_term>

				#clean multiple spaces
                WHILE instr(@Keywords, '  ') > 0 do
					SET @Keywords = REPLACE(@Keywords, '  ', ' ');
				end while;

				IF `FullTextMode` = 5 then #5 - using CONTAINS and OR with <prefix_term>
					SET @concat_term = ' ';
				END if;
				IF `FullTextMode` = 10 then #10 - using CONTAINS and AND with <prefix_term>
					SET @concat_term = ' +';
				END if;
                
                #now let's build search string
				set @fulltext_keywords = '';
				set @str_index = instr(@Keywords, ' ');
                
                # if index = 0, then only one field was passed
				IF(@str_index = 0) then
					set @fulltext_keywords = concat(' "', @Keywords, '*" ');
				ELSE
					set @fulltext_keywords = replace(@Keywords, ' ', @concat_term);
                end if;
                SET @Keywords = @fulltext_keywords;
            end if;
        end if;
        
        #product name
		SET @sql_command = '
		INSERT INTO `KeywordProducts` (ProductId)
		SELECT p.Id
		FROM Product p
		WHERE ';
        
		IF UseFullTextSearch then
			SET @sql_command = concat(@sql_command, 'MATCH (p.`Name`) AGAINST (@Keywords IN BOOLEAN MODE) ');
		ELSE
			SET @sql_command = concat(@sql_command, 'instr(p.Name, @Keywords) > 0 ');
        end if;
        
        #localized product name        
		SET @sql_command = concat(@sql_command, '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp
		WHERE
			lp.LocaleKeyGroup = ''Product''
			AND lp.LanguageId = ', `LanguageId`, '
			AND lp.LocaleKey = ''Name''');
            
		IF UseFullTextSearch = 1 then
			SET @sql_command = concat(@sql_command, ' AND MATCH (lp.LocaleValue) AGAINST (@Keywords IN BOOLEAN MODE) ');
		ELSE
			SET @sql_command = concat(@sql_command, ' AND instr(lp.LocaleValue, @Keywords) > 0 ');
		end if;
        
        if `SearchDescriptions` then
			#product short description
			SET @sql_command = concat(@sql_command, '
			UNION
			SELECT p.Id
			FROM Product p
			WHERE ');
            
			IF `UseFullTextSearch` then
				SET @sql_command = concat(@sql_command, 'MATCH (p.ShortDescription, p.FullDescription) AGAINST (@Keywords IN BOOLEAN MODE) ');
			ELSE
				SET @sql_command = concat(@sql_command, 'instr(p.ShortDescription, @Keywords) > 0 or instr(p.FullDescription, @Keywords) > 0 ');
			end if;

			#localized product short description
			SET @sql_command = concat(@sql_command, '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp
			WHERE
				lp.LocaleKeyGroup = ''Product''
				AND lp.LanguageId = ', `LanguageId`, '
				AND lp.LocaleKey = ''ShortDescription''');
                
			IF `UseFullTextSearch` then
				SET @sql_command =  concat(@sql_command, ' AND MATCH (lp.LocaleValue) AGAINST (@Keywords IN BOOLEAN MODE) ');
			ELSE
				SET @sql_command = concat(@sql_command, ' AND instr(lp.LocaleValue, @Keywords) > 0 ');
			end if;

			#localized product full description
			SET @sql_command = concat(@sql_command, '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp
			WHERE
				lp.LocaleKeyGroup = N''Product''
				AND lp.LanguageId = ', `LanguageId`, '
				AND lp.LocaleKey = N''FullDescription''');
                
			IF `UseFullTextSearch` then
				SET @sql_command = concat(@sql_command, ' AND MATCH (lp.LocaleValue) AGAINST (@Keywords IN BOOLEAN MODE) ');
			ELSE
				SET @sql_command = concat(@sql_command, ' AND instr(lp.LocaleValue, @Keywords) > 0 ');
			end if;
            
            
        end if;
        
        #manufacturer part number (exact match)
		IF `SearchManufacturerPartNumber` then
			SET @sql_command = concat(@sql_command, '
			UNION
			SELECT p.Id
			FROM Product p
			WHERE p.ManufacturerPartNumber = @OriginalKeywords ');
		END if;

		#SKU (exact match)
		IF `SearchSku` then
			SET @sql_command = concat(@sql_command, '
			UNION
			SELECT p.Id
			FROM Product p
			WHERE p.Sku = @OriginalKeywords ');
		END if;
        
        IF `SearchProductTags` then
			#product tags (exact match)
			SET @sql_command = concat(@sql_command, '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm INNER JOIN ProductTag pt ON pt.Id = pptm.ProductTag_Id
			WHERE pt.`Name` = @OriginalKeywords ');

			#localized product tags
			SET @sql_command = concat(@sql_command, '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp INNER JOIN Product_ProductTag_Mapping pptm ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ', `LanguageId`, '
				AND lp.LocaleKey = N''Name''
				AND lp.`LocaleValue` = @OriginalKeywords ');
		END if;
        
        #select  @sql_command, @Keywords, @OriginalKeywords; #debug
        
        PREPARE sql_stmts FROM @sql_command;
		EXECUTE sql_stmts;
		DEALLOCATE PREPARE sql_stmts;
    end if;
    
    create temporary table `DisplayOrderTmp`
	(
		Id int NOT NULL auto_increment,
		ProductId int NOT NULL,
        PRIMARY KEY (id)
	);
    
    #filter by category IDs
	SET `CategoryIds` = COALESCE(`CategoryIds`, '');
	
    SET @sql_command = '
	SELECT p.Id
	FROM
		Product p';
    
    IF `CategoryIds` REGEXP '^([[:digit:]](,?))+$' then
		SET @sql_command = concat(@sql_command, '
		INNER JOIN Product_Category_Mapping pcm
			ON p.Id = pcm.ProductId');
	END if;
    
    IF `ManufacturerId` > 0 then
		SET @sql_command = concat(@sql_command, '
		INNER JOIN Product_Manufacturer_Mapping pmm
			ON p.Id = pmm.ProductId');
	END if;
    
    IF COALESCE(`ProductTagId`, 0) != 0 then
		SET @sql_command = concat(@sql_command, '
		INNER JOIN Product_ProductTag_Mapping pptm
			ON p.Id = pptm.Product_Id');
	END if;
    
    #searching by keywords
	IF `SearchKeywords` then
		SET @sql_command = concat(@sql_command, '
		JOIN `KeywordProducts` kp
			ON  p.Id = kp.ProductId');
	END if;
    
    #filter by vendor online
	IF `VendorOnline` IS NOT NULL then
		SET  @sql_command = concat(@sql_command, '
		JOIN Vendor vd 
			ON vd.Id = p.VendorId');
	END if;
    
    SET @sql_command = concat(@sql_command, '
		WHERE
			p.Deleted = 0');
		
	IF `VendorOnline` IS NOT NULL then
		SET  @sql_command = concat(@sql_command, '
		AND vd.Active = ', `VendorOnline`);
	END if;
	IF `VendorOnline` IS NOT NULL then
		SET  @sql_command = concat(@sql_command, '
		AND vd.Online = ', `VendorOnline`);
	END if;
    
    #filter by category
	IF `CategoryIds` REGEXP '^([[:digit:]](,?))+$' then
		SET @sql_command = concat(@sql_command, '
		AND pcm.CategoryId IN (', `CategoryIds`, ')');		

		IF `FeaturedProducts` IS NOT NULL then
			SET @sql_command = concat(@sql_command, '
				AND pcm.IsFeaturedProduct = ', `FeaturedProducts`);
		END if;
	END if;
    
    #filter by manufacturer
	IF `ManufacturerId` > 0 then
		SET  @sql_command = concat(@sql_command, '
			AND pmm.ManufacturerId = ', `ManufacturerId`);
		
		IF `FeaturedProducts` IS NOT NULL then
			SET  @sql_command = concat(@sql_command, '
				AND pmm.IsFeaturedProduct = ', `FeaturedProducts`);
		END if;
	END if;
    
    #filter by vendor
	IF `VendorId` > 0 then
		SET  @sql_command = concat(@sql_command, '
		AND p.VendorId = ', `VendorId`);
	END if;
    
    #filter by warehouse
	IF `WarehouseId` > 0 then
		#we should also ensure that 'ManageInventoryMethodId' is set to 'ManageStock' (1)
		#but we skip it in order to prevent hard-coded values (e.g. 1) and for better performance
		SET  @sql_command = concat(@sql_command, '
		AND  
			(
				(p.UseMultipleWarehouses = 0 AND
					p.WarehouseId = ', `WarehouseId`, ')
				OR
				(p.UseMultipleWarehouses > 0 AND
					EXISTS (SELECT 1 FROM ProductWarehouseInventory pwi
					WHERE pwi.WarehouseId = ', `WarehouseId`, ' AND pwi.ProductId = p.Id))
			)');
	END if;
    
    #filter by product type
	IF `ProductTypeId` is not null then
		SET  @sql_command = concat(@sql_command, '
			AND p.ProductTypeId = ', `ProductTypeId`);
	END if;
    
    #filter by product type
	IF `ProductApprovalStatusId` is not null then
		SET  @sql_command = concat(@sql_command, '
			AND p.ProductApprovalStatusId = ', `ProductApprovalStatusId`);
	END if;
	
	#filter by "visible individually"
	IF `VisibleIndividuallyOnly` then
		SET  @sql_command = concat(@sql_command, '
			AND p.VisibleIndividually = 1');
	END if;
	
	#filter by "marked as new"
	IF `MarkedAsNewOnly` then
		SET  @sql_command = concat(@sql_command, '
			AND p.MarkAsNew = 1
			AND (utc_date() BETWEEN IFNULL(p.MarkAsNewStartDateTimeUtc, ''1900-1-1'') and IFNULL(p.MarkAsNewEndDateTimeUtc, ''2999-1-1''))');
	END if;
    
    #filter by product tag
	IF COALESCE(`ProductTagId`, 0) != 0 then
		SET  @sql_command = concat(@sql_command, '
			AND pptm.ProductTag_Id = ', `ProductTagId`);
	END if;
	
	#"Published" property
	IF `OverridePublished` is null then
		#process according to "showHidden"
		IF not `ShowHidden` then
			SET  @sql_command = concat(@sql_command, '
			AND p.Published');
		END if;
	ELSEIF `OverridePublished` then
		#published only
		SET  @sql_command = concat(@sql_command, '
			AND p.Published');
	ELSEIF not `OverridePublished` then
		#unpublished only
		SET  @sql_command = concat(@sql_command, '
			AND not p.Published');
	END if;
	
	#show hidden
	IF not `ShowHidden` then
		SET  @sql_command = concat(@sql_command, '
			AND not p.Deleted
			AND (utc_date() BETWEEN IFNULL(p.AvailableStartDateTimeUtc, ''1000-01-01'') and IFNULL(p.AvailableEndDateTimeUtc, ''9999-12-31''))');
	END if;
    
    #min price
	IF `PriceMin` is not null then
		SET  @sql_command = concat(@sql_command, '
			AND (p.Price >= ', `PriceMin`, ')');
	END if;
	
	#max price
	IF `PriceMax` is not null then
		SET  @sql_command = concat(@sql_command, '
		AND (p.Price <= ', `PriceMax`, ')');
	END if;
    
    #show hidden and ACL
	IF not `ShowHidden` and `AllowedCustomerRoleIds` REGEXP '^([[:digit:]](,?))+$' then
		SET  @sql_command = concat(@sql_command, '
			AND (not p.SubjectToAcl OR EXISTS (
					SELECT 1 
					from aclRecord as acl 
					where acl.CustomerRoleId in (', `AllowedCustomerRoleIds` ,') 
						and acl.`EntityId` = p.`Id` AND acl.`EntityName` = ''Product''
					)
				)');
	END if;

	#filter by store
	IF `StoreId` > 0 then
		SET  @sql_command = concat(@sql_command, '
			AND (not p.LimitedToStores OR EXISTS (
				SELECT 1 FROM StoreMapping sm
				WHERE sm.EntityId = p.Id AND sm.EntityName = ''Product'' and sm.StoreId=', `StoreId`, '
				))');
	END if;
    
    #prepare filterable specification attribute option identifier (if requested)
    IF `LoadFilterableSpecificationAttributeOptionIds` then
        SET @sql_filterableSpecs = concat('
	        SELECT group_concat(DISTINCT `psam`.SpecificationAttributeOptionId separator '','')
	        FROM `Product_SpecificationAttribute_Mapping` `psam`
	            WHERE `psam`.`AllowFiltering`
	            AND `psam`.`ProductId` IN (', @sql_command, ') into @FilterableSpecs');
                
		#select  @sql_filterableSpecs; #debug
        PREPARE sql_filterableSpecs_stmts FROM @sql_filterableSpecs;
		EXECUTE sql_filterableSpecs_stmts;
		
		#build comma separated list of filterable identifiers
        if @FilterableSpecs is not null and length(@FilterableSpecs) > 0 then
			Set `FilterableSpecificationAttributeOptionIds` = concat(IFNULL(concat(`FilterableSpecificationAttributeOptionIds`, ','), ''), @FilterableSpecs);
        end if;
        
		DEALLOCATE PREPARE sql_filterableSpecs_stmts;
    end if;
    
    #filter by specification attribution options
    IF `FilteredSpecs` REGEXP '^([[:digit:]](,?))+$' then
		SET  @sql_command = concat(@sql_command, '
			AND (p.Id in (
					select psa.ProductId 
					from `Product_SpecificationAttribute_Mapping` as psa 
                    INNER JOIN (
						select sao.Id 
						from `SpecificationAttributeOption` as sao 
						where sao.Id in (', `FilteredSpecs`, ')
						) as sa on sa.Id = psa.SpecificationAttributeOptionId
                )
			)');
    end if;
    
    #rating
	IF `RatingMin` is not null AND `RatingMax` is not null then
		SET  @sql_command = concat(@sql_command, 
		' AND (
			IFNULL((SELECT
			SUM(CASE WHEN pr.IsApproved = 1 THEN pr.Rating ELSE 0 END)
			 / SUM(CASE WHEN pr.IsApproved = 1 THEN 1 ELSE 0 END)
			FROM ProductReview pr
			WHERE pr.ProductId = p.Id),1) BETWEEN ', `RatingMin`, ' AND ', `RatingMax`, ')');
	END if;
    
    #location filter
	IF `Longitude` is not null AND `Latitude` is not null AND `CoverageDistance` is not null
	then
		SET  @sql_command = concat(@sql_command, 
		' AND (
			(SELECT
			(
				3959 
				* acos (
					cos ( radians(',`Latitude`,') )
					* cos( radians( a.Latitude ) )
					* cos( radians( a.Longitude ) - radians(',`Longitude`,') )
					+ sin ( radians(',`Latitude`,') )
					* sin( radians( a.Latitude ) )
				)
			)
			FROM Vendor v
			LEFT JOIN Address a
			ON a.Id = v.AddressId
			WHERE v.Id = p.VendorId) <= ', `CoverageDistance`, ')');
	END if;

	#sorting
    SET @sql_orderby = '';
    
    CASE `OrderBy` 
	WHEN 5 THEN Set @sql_orderby = ' p.`Name` ASC'; /* Name: A to Z */
	WHEN 6 THEN Set @sql_orderby = ' p.`Name` DESC'; /* Name: Z to A */
	WHEN 10 THEN Set @sql_orderby = ' p.`Price` ASC'; /* Price: Low to High */
	WHEN 11 THEN Set @sql_orderby = ' p.`Price` DESC'; /* Price: High to Low */
	WHEN 15 THEN Set @sql_orderby = ' p.`CreatedOnUtc` DESC'; /* creation date */
	ELSE /* default sorting, 0 (position) */
		begin
			IF `CategoryIds` REGEXP '^([[:digit:]](,?))+$' then 
				SET @sql_orderby = ' pcm.DisplayOrder ASC';
			end if;
            
			#manufacturer position (display order)
			IF `ManufacturerId` > 0 then
				IF length(@sql_orderby) > 0 then 
					SET @sql_orderby = concat(@sql_orderby, ', ');
				end if;
				SET @sql_orderby =  concat(@sql_orderby, ' pmm.DisplayOrder ASC');
			END if;
			
			#name
			IF length(@sql_orderby) > 0 then 
				SET @sql_orderby = concat(@sql_orderby, ', ');
			end if;
			SET @sql_orderby = concat(@sql_orderby,  ' p.`Name` ASC');
        end;
	end case;
    
    SET @sql_command = concat(@sql_command, '
	ORDER BY', @sql_orderby);
    
    SET @sql_command = concat('
    INSERT INTO DisplayOrderTmp (ProductId)', @sql_command);
    
    #SELECT @sql_command; #debug
    
    PREPARE sql_do_stmts FROM @sql_command;
	EXECUTE sql_do_stmts;
	DEALLOCATE PREPARE sql_do_stmts;
    
    select count(Id) from `DisplayOrderTmp` into `TotalRecords`;
    
    #return products
	SELECT p.*
	FROM `DisplayOrderTmp` dot
		INNER JOIN Product p on p.Id = dot.ProductId
	WHERE dot.Id > `PageSize` * `PageIndex`
	ORDER BY dot.Id
    limit `PageSize`;
    
    drop temporary TABLE if exists `KeywordProducts`;
    drop temporary TABLE if exists `DisplayOrderTmp`;
END$$
DELIMITER ;