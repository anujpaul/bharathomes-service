-- Agents (these also need a UserProfile row first due to FK constraint)
INSERT INTO "UserProfiles"
("Id", "Name", "Email", "Phone", "PasswordHash", "UserPhoto", "Provider", "AccountStatus", "UserType")
VALUES 
('a1', 'Sunny Paul', '', '975-959-8991', NULL, 'sunny_paul.jpg', 'local', true, 'agent'),
('a2', 'Priya Verma', '', '', NULL, 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=400&q=80', 'local', true, 'agent');

INSERT INTO Agents (Id, Rating, ListingsCount, Specialization, ReraRegistrationNumber, OperatingLocation)
VALUES 
('a1', 4.9, 45, 'Apartments, Villas & Plots, Agra and Noida', '', NULL),
('a2', 4.8, 32, 'Luxury Apartments, Villas & Plots, Noida', '', NULL);
INSERT INTO "Agents"
("Id", "Rating", "ListingsCount", "Specialization", "ReraRegistrationNumber", "OperatingLocation")
VALUES 
('a1', 4.9, 45, 'Apartments, Villas & Plots, Agra and Noida', '', NULL),
('a2', 4.8, 32, 'Luxury Apartments, Villas & Plots, Noida', '', NULL);

-- Properties
INSERT INTO "Properties"
("Id", "Title", "Price", "Location", "City", "Beds", "Baths", "Sqft", "Type",
 "IsFeatured", "ExpresswayProximity", "IsReraRegistered",
 "ReraRegistrationNumber", "VastuOrientation", "CreatedAt")
VALUES
('1', 'Luxury 4BHK Penthouse', 45000000, 'Sector 150, Noida Expressway', 'Noida', 4, 4, 3200, 'Apartment', true, true, false, NULL, NULL, NOW()),
('2', 'Modern Villa with Taj View', 28000000, 'Fatehabad Road, Agra', 'Agra', 3, 3, 2400, 'Villa', true, false, false, NULL, NULL, NOW()),
('3', 'Premium Studio Apartment', 6500000, 'Knowledge Park III, Greater Noida', 'Greater Noida', 1, 1, 650, 'Apartment', false, true, false, NULL, NULL, NOW()),
('4', 'Premium Apartment', 26500000, 'Knowledge Park III, Greater Noida', 'Greater Noida', 3, 2, 1650, 'Apartment', false, true, false, NULL, NULL, NOW()),
('5', 'Residential Plot near Yamuna Expressway', 8500000, 'Sector 22D, Greater Noida', 'Greater Noida', 0, 0, 1800, 'Plot', true, true, false, NULL, NULL, NOW()),
('6', '3BHK Family Home', 15000000, 'Shastri Puram, Agra', 'Agra', 3, 2, 1800, 'Apartment', false, false, false, NULL, NULL, NOW()),
('8', 'Commercial Office Space', 12000000, 'Sector 62, Noida', 'Noida', 0, 2, 1200, 'Commercial', false, false, false, NULL, NULL, NOW());

-- Property Images
INSERT INTO "PropertyImages"
("Url", "Order", "PropertyId")
VALUES
('https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80', 0, '1'),
('https://images.unsplash.com/photo-1613490493576-7fde63acd811?auto=format&fit=crop&w=800&q=80', 1, '1'),
('https://images.unsplash.com/photo-1523217582562-09d0def993a6?auto=format&fit=crop&w=800&q=80', 2, '1'),
('https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=800&q=80', 3, '1'),
('https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=800&q=80', 4, '1'),
('https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?auto=format&fit=crop&w=800&q=80', 5, '1'),
('https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=800&q=80', 6, '1'),
('https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?auto=format&fit=crop&w=800&q=80', 7, '1'),
('https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea?auto=format&fit=crop&w=800&q=80', 8, '1'),
('https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=800&q=80', 9, '1'),
('https://images.unsplash.com/photo-1600585152220-90363fe7e115?auto=format&fit=crop&w=800&q=80', 10, '1'),
('https://images.unsplash.com/photo-1592229505726-ca121723b8ef?auto=format&fit=crop&w=800&q=80', 11, '1'),
('https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=800&q=80', 12, '1'),
('https://images.unsplash.com/photo-1600607687644-aac4c3eac7f4?auto=format&fit=crop&w=800&q=80', 13, '1'),
('https://images.unsplash.com/photo-1600585154526-990dced4db0d?auto=format&fit=crop&w=800&q=80', 14, '1'),
-- Property 2
('https://images.unsplash.com/photo-1613490493576-7fde63acd811?auto=format&fit=crop&w=800&q=80', 0, '2'),
('https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=800&q=80', 1, '2'),
('https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?auto=format&fit=crop&w=800&q=80', 2, '2'),
('https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=800&q=80', 3, '2'),
-- Property 3
('https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80', 0, '3'),
-- Property 4
('https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80', 0, '4'),
-- Property 5
('https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=800&q=80', 0, '5'),
-- Property 6
('https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=800&q=80', 0, '6'),
-- Property 8
('https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=800&q=80', 0, '8');

-- Property Agents
INSERT INTO "PropertyAgents"
("PropertyId", "AgentId")
VALUES
('1', 'a2');


INSERT INTO PropertyAgents (PropertyId, AgentId)
VALUES ('1', 'a2');



-- PostGresDB:===========

-- Agents
INSERT INTO "Agents"
("Id", "Rating", "ListingsCount", "Specialization", "ReraRegistrationNumber", "OperatingLocation")
VALUES 
('a1', 4.9, 45, 'Apartments, Villas & Plots, Agra and Noida', '', NULL),
('a2', 4.8, 32, 'Luxury Apartments, Villas & Plots, Noida', '', NULL);

-- Properties
INSERT INTO "Properties"
("Id", "Title", "Price", "Location", "City", "Beds", "Baths", "Sqft", "Type",
 "IsFeatured", "ExpresswayProximity", "IsReraRegistered",
 "ReraRegistrationNumber", "VastuOrientation", "CreatedAt")
VALUES
('1', 'Luxury 4BHK Penthouse', 45000000, 'Sector 150, Noida Expressway', 'Noida', 4, 4, 3200, 'Apartment', true, true, false, NULL, NULL, NOW()),
('2', 'Modern Villa with Taj View', 28000000, 'Fatehabad Road, Agra', 'Agra', 3, 3, 2400, 'Villa', true, false, false, NULL, NULL, NOW()),
('3', 'Premium Studio Apartment', 6500000, 'Knowledge Park III, Greater Noida', 'Greater Noida', 1, 1, 650, 'Apartment', false, true, false, NULL, NULL, NOW()),
('4', 'Premium Apartment', 26500000, 'Knowledge Park III, Greater Noida', 'Greater Noida', 3, 2, 1650, 'Apartment', false, true, false, NULL, NULL, NOW()),
('5', 'Residential Plot near Yamuna Expressway', 8500000, 'Sector 22D, Greater Noida', 'Greater Noida', 0, 0, 1800, 'Plot', true, true, false, NULL, NULL, NOW()),
('6', '3BHK Family Home', 15000000, 'Shastri Puram, Agra', 'Agra', 3, 2, 1800, 'Apartment', false, false, false, NULL, NULL, NOW()),
('8', 'Commercial Office Space', 12000000, 'Sector 62, Noida', 'Noida', 0, 2, 1200, 'Commercial', false, false, false, NULL, NULL, NOW());

-- Property Images
INSERT INTO "PropertyImages"
("Url", "Order", "PropertyId")
VALUES
('https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80', 0, '1'),
('https://images.unsplash.com/photo-1613490493576-7fde63acd811?auto=format&fit=crop&w=800&q=80', 1, '1'),
('https://images.unsplash.com/photo-1523217582562-09d0def993a6?auto=format&fit=crop&w=800&q=80', 2, '1'),
('https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=800&q=80', 3, '1'),
('https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=800&q=80', 4, '1'),
('https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?auto=format&fit=crop&w=800&q=80', 5, '1'),
('https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=800&q=80', 6, '1'),
('https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?auto=format&fit=crop&w=800&q=80', 7, '1'),
('https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea?auto=format&fit=crop&w=800&q=80', 8, '1'),
('https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=800&q=80', 9, '1'),
('https://images.unsplash.com/photo-1600585152220-90363fe7e115?auto=format&fit=crop&w=800&q=80', 10, '1'),
('https://images.unsplash.com/photo-1592229505726-ca121723b8ef?auto=format&fit=crop&w=800&q=80', 11, '1'),
('https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=800&q=80', 12, '1'),
('https://images.unsplash.com/photo-1600607687644-aac4c3eac7f4?auto=format&fit=crop&w=800&q=80', 13, '1'),
('https://images.unsplash.com/photo-1600585154526-990dced4db0d?auto=format&fit=crop&w=800&q=80', 14, '1'),
-- Property 2
('https://images.unsplash.com/photo-1613490493576-7fde63acd811?auto=format&fit=crop&w=800&q=80', 0, '2'),
('https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=800&q=80', 1, '2'),
('https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?auto=format&fit=crop&w=800&q=80', 2, '2'),
('https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=800&q=80', 3, '2'),
-- Property 3
('https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80', 0, '3'),
-- Property 4
('https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80', 0, '4'),
-- Property 5
('https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=800&q=80', 0, '5'),
-- Property 6
('https://images.unsplash.com/photo-1484154218962-a197022b5858?auto=format&fit=crop&w=800&q=80', 0, '6'),
-- Property 8
('https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=800&q=80', 0, '8');

-- Property Agents
INSERT INTO "PropertyAgents"
("PropertyId", "AgentId")
VALUES
('1', 'a2');

select * from "Properties"
select * from "PropertyImage"


dotnet ef migrations remove 
Remove-Item Migrations -Recurse -Force

dotnet ef migrations add InitialCreate

dotnet ef database update