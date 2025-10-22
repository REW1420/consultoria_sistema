import {
  IsBoolean,
  IsInt,
  IsNotEmpty,
  IsNumber,
  IsOptional,
  IsString,
  Max,
  Min,
} from "class-validator";
import { PartialType } from "../../utils/partial-type";

export class CreateCarDto {
  @IsString()
  @IsNotEmpty()
  model!: string;

  @IsString()
  @IsOptional()
  description?: string;

  @IsInt()
  @Min(1990)
  @Max(2100)
  year!: number;

  @IsNumber()
  price!: number;

  @IsOptional()
  @IsString()
  image_url?: string;

  @IsOptional()
  @IsInt()
  transmission_id?: number;

  @IsOptional()
  @IsInt()
  condition_id?: number;

  @IsOptional()
  @IsInt()
  created_by?: number;
}

export class UpdateCarDto {
  @IsOptional()
  @IsString()
  model?: string;

  @IsOptional()
  @IsString()
  description?: string;

  @IsOptional()
  @IsInt()
  year?: number;

  @IsOptional()
  @IsNumber()
  price?: number;

  @IsOptional()
  @IsString()
  image_url?: string;

  @IsOptional()
  @IsBoolean()
  is_published?: boolean;

  @IsOptional()
  @IsInt()
  transmission_id?: number;

  @IsOptional()
  @IsInt()
  condition_id?: number;

  @IsOptional()
  @IsInt()
  created_by?: number;
}
