-- 1. Unificar Responsáveis com o mesmo Documento (CPF)
UPDATE "Alunos"
SET "ResponsavelId" = sub."primeiro_id"
FROM (
    SELECT "Documento", MIN("Id"::text)::uuid as primeiro_id
    FROM "Responsaveis"
    WHERE "Documento" IS NOT NULL AND "Documento" != ''
    GROUP BY "Documento"
    HAVING COUNT(*) > 1
) sub
JOIN "Responsaveis" r ON r."Documento" = sub."Documento"
WHERE "Alunos"."ResponsavelId" = r."Id"
  AND r."Id" != sub."primeiro_id";

DELETE FROM "Responsaveis"
WHERE "Id" NOT IN (
    SELECT MIN("Id"::text)::uuid
    FROM "Responsaveis"
    GROUP BY "Documento"
) AND "Documento" IS NOT NULL AND "Documento" != '';

-- 2. Unificar Responsáveis com o mesmo Email
UPDATE "Alunos"
SET "ResponsavelId" = sub."primeiro_id"
FROM (
    SELECT "Email", MIN("Id"::text)::uuid as primeiro_id
    FROM "Responsaveis"
    WHERE "Email" IS NOT NULL AND "Email" != ''
    GROUP BY "Email"
    HAVING COUNT(*) > 1
) sub
JOIN "Responsaveis" r ON r."Email" = sub."Email"
WHERE "Alunos"."ResponsavelId" = r."Id"
  AND r."Id" != sub."primeiro_id";

DELETE FROM "Responsaveis"
WHERE "Id" NOT IN (
    SELECT MIN("Id"::text)::uuid
    FROM "Responsaveis"
    GROUP BY "Email"
) AND "Email" IS NOT NULL AND "Email" != '';
